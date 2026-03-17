好的，下面我将为您提供一个完整的前后端代码示例，实现使用 C# Web API 提供一个几 GB 的 ZIP 压缩文件的断点续传功能，前端使用 JavaScript 实现下载和断点续传。

## 后端：C# Web API 实现

### 1. 创建控制器

首先，在您的 ASP.NET Core Web API 项目中创建一个控制器，例如 `DownloadController`。

```csharp
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using Microsoft.Net.Http.Headers;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        [HttpGet("largefile")]
        public IActionResult DownloadLargeFile()
        {
            var filePath = "path/to/your/largefile.zip";
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileSize = new FileInfo(filePath).Length;
            var rangeHeader = Request.Headers["Range"].ToString();

            if (string.IsNullOrEmpty(rangeHeader))
            {
                // 未提供 Range 请求头，返回整个文件
                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(stream, "application/zip", "largefile.zip", enableRangeProcessing: true);
            }
            else
            {
                // 解析 Range 请求头
                var range = RangeHeaderValue.Parse(rangeHeader);
                var from = range.Ranges.First().From ?? 0;
                var to = range.Ranges.First().To ?? (fileSize - 1);

                if (from >= fileSize || to >= fileSize)
                {
                    return StatusCode(416); // Range Not Satisfiable
                }

                var contentLength = to - from + 1;

                // 设置响应头
                Response.StatusCode = StatusCodes.Status206PartialContent;
                Response.Headers["Content-Range"] = $"bytes {from}-{to}/{fileSize}";
                Response.Headers["Accept-Ranges"] = "bytes";
                Response.Headers["Content-Length"] = contentLength.ToString();

                // 返回文件流
                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                stream.Seek(from, SeekOrigin.Begin);

                return new FileStreamResult(stream, "application/zip")
                {
                    EnableRangeProcessing = true,
                    FileDownloadName = "largefile.zip"
                };
            }
        }
    }
}
```

#### 代码说明

- **Range 处理**：检查请求头中是否包含 `Range`，如果有，则解析请求的范围。
- **响应状态码**：对于部分内容，返回 `206 Partial Content`。
- **响应头设置**：
  - `Content-Range`：指明返回的字节范围和文件总大小。
  - `Accept-Ranges`：告知客户端服务器支持字节范围请求。
  - `Content-Length`：当前响应内容的长度。
- **文件流**：使用 `FileStream` 和 `Seek` 方法，从指定位置开始读取文件。

### 2. 启用静态文件服务（可选）

如果您希望直接使用静态文件中间件处理，可以在 `Startup.cs` 中配置：

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // 其他配置...

    app.UseStaticFiles(new StaticFileOptions
    {
        ServeUnknownFileTypes = true,
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers["Accept-Ranges"] = "bytes";
        }
    });

    // 其他配置...
}
```

但由于您的文件可能不在 `wwwroot` 目录下，还是推荐使用控制器方式。

## 前端：JavaScript 实现

### 1. 基本思路

- **断点续传**：通过 `Range` 请求头指定下载的字节范围。
- **保存进度**：在客户端记录已下载的字节数，以便在中断后继续下载。
- **文件保存**：由于浏览器的限制，需要使用 `Blob` 对象并利用 `FileSaver` 等库触发下载。

### 2. 实现代码

首先，确保引入了 `FileSaver.js`（可以通过 npm 或直接在 HTML 中引入 CDN）。

#### HTML 部分

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>断点续传下载示例</title>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/FileSaver.js/2.0.5/FileSaver.min.js"></script>
</head>
<body>
    <button id="downloadBtn">下载文件</button>
    <script src="script.js"></script>
</body>
</html>
```

#### JavaScript 部分（`script.js`）

```javascript
const downloadUrl = 'http://localhost:5000/api/download/largefile';
const chunkSize = 1024 * 1024; // 每次请求 1MB
let receivedLength = 0; // 已接收的字节数
let chunks = []; // 存储每个块
let fileSize = 0;

document.getElementById('downloadBtn').addEventListener('click', () => {
    startDownload();
});

async function startDownload() {
    // 首先获取文件的总大小
    fileSize = await getFileSize();

    // 开始下载
    while (receivedLength < fileSize) {
        const rangeStart = receivedLength;
        const rangeEnd = Math.min(receivedLength + chunkSize - 1, fileSize - 1);
        const chunk = await downloadChunk(rangeStart, rangeEnd);
        chunks.push(chunk);
        receivedLength += chunk.byteLength;
        console.log(`已下载：${((receivedLength / fileSize) * 100).toFixed(2)}%`);
    }

    // 合并所有块并保存文件
    const blob = new Blob(chunks, { type: 'application/zip' });
    saveAs(blob, 'largefile.zip');
}

async function getFileSize() {
    const response = await fetch(downloadUrl, { method: 'HEAD' });
    if (!response.ok) {
        throw new Error('无法获取文件大小');
    }
    return parseInt(response.headers.get('Content-Length'), 10);
}

async function downloadChunk(start, end) {
    const response = await fetch(downloadUrl, {
        headers: {
            'Range': `bytes=${start}-${end}`
        }
    });
    if (response.status !== 206 && response.status !== 200) {
        throw new Error('下载失败');
    }
    return await response.arrayBuffer();
}
```

#### 代码说明

- **getFileSize**：通过发送一个 `HEAD` 请求获取文件的总大小。
- **downloadChunk**：发送带有 `Range` 请求头的请求，获取指定范围的字节。
- **startDownload**：循环下载文件的每个块，直到完成。
- **进度显示**：在控制台中输出已下载的百分比。
- **文件保存**：使用 `Blob` 对象将所有块合并，并使用 `FileSaver.js` 保存文件。

### 3. 处理断点续传

如果您希望在下载中断后能够继续下载，需要将 `receivedLength` 和 `chunks` 存储在本地存储（`localStorage`）或索引数据库（`IndexedDB`）中。

以下是修改后的代码，添加了简单的断点续传功能：

```javascript
const downloadUrl = 'http://localhost:5000/api/download/largefile';
const chunkSize = 1024 * 1024; // 每次请求 1MB
let receivedLength = 0; // 已接收的字节数
let chunks = []; // 存储每个块
let fileSize = 0;

document.getElementById('downloadBtn').addEventListener('click', () => {
    startDownload();
});

async function startDownload() {
    // 尝试从本地存储恢复进度
    loadProgress();

    // 获取文件的总大小
    fileSize = await getFileSize();

    // 开始下载
    while (receivedLength < fileSize) {
        const rangeStart = receivedLength;
        const rangeEnd = Math.min(receivedLength + chunkSize - 1, fileSize - 1);
        const chunk = await downloadChunk(rangeStart, rangeEnd);
        chunks.push(chunk);
        receivedLength += chunk.byteLength;
        console.log(`已下载：${((receivedLength / fileSize) * 100).toFixed(2)}%`);

        // 保存进度
        saveProgress();
    }

    // 下载完成，清除进度并保存文件
    clearProgress();
    const blob = new Blob(chunks, { type: 'application/zip' });
    saveAs(blob, 'largefile.zip');
}

function saveProgress() {
    localStorage.setItem('downloadedLength', receivedLength);
    localStorage.setItem('chunks', JSON.stringify(chunks.map(chunk => Array.from(new Uint8Array(chunk)))));
}

function loadProgress() {
    const savedLength = localStorage.getItem('downloadedLength');
    const savedChunks = localStorage.getItem('chunks');

    if (savedLength && savedChunks) {
        receivedLength = parseInt(savedLength, 10);
        const chunksArray = JSON.parse(savedChunks);
        chunks = chunksArray.map(arr => new Uint8Array(arr).buffer);
        console.log('恢复下载进度');
    }
}

function clearProgress() {
    localStorage.removeItem('downloadedLength');
    localStorage.removeItem('chunks');
}

async function getFileSize() {
    const response = await fetch(downloadUrl, { method: 'HEAD' });
    if (!response.ok) {
        throw new Error('无法获取文件大小');
    }
    return parseInt(response.headers.get('Content-Length'), 10);
}

async function downloadChunk(start, end) {
    const response = await fetch(downloadUrl, {
        headers: {
            'Range': `bytes=${start}-${end}`
        }
    });
    if (response.status !== 206 && response.status !== 200) {
        throw new Error('下载失败');
    }
    return await response.arrayBuffer();
}
```

#### 注意事项

- **本地存储限制**：`localStorage` 对于大型数据并不适用，存储容量有限。对于更大的数据，建议使用 `IndexedDB`。
- **数据序列化**：由于 `ArrayBuffer` 无法直接序列化，需要转换为数组。

### 4. 使用 IndexedDB（高级）

为了更可靠地存储大文件的下载进度，可以使用 `IndexedDB`。以下是使用 `IndexedDB` 的示例：

#### 初始化 IndexedDB

```javascript
let db;
const request = indexedDB.open('DownloadDB', 1);

request.onupgradeneeded = event => {
    db = event.target.result;
    if (!db.objectStoreNames.contains('chunks')) {
        db.createObjectStore('chunks', { autoIncrement: true });
    }
};

request.onsuccess = event => {
    db = event.target.result;
};

request.onerror = event => {
    console.error('IndexedDB 错误：', event.target.errorCode);
};
```

#### 存储和加载进度

```javascript
function saveChunk(chunk) {
    const transaction = db.transaction(['chunks'], 'readwrite');
    const store = transaction.objectStore('chunks');
    store.add(chunk);
}

function getAllChunks() {
    return new Promise((resolve, reject) => {
        const transaction = db.transaction(['chunks'], 'readonly');
        const store = transaction.objectStore('chunks');
        const request = store.getAll();
        request.onsuccess = () => {
            resolve(request.result);
        };
        request.onerror = () => {
            reject('无法获取已下载的块');
        };
    });
}

function clearChunks() {
    const transaction = db.transaction(['chunks'], 'readwrite');
    const store = transaction.objectStore('chunks');
    store.clear();
}
```

#### 在下载过程中使用

```javascript
async function startDownload() {
    // 获取文件的总大小
    fileSize = await getFileSize();

    // 尝试加载已下载的块
    const savedChunks = await getAllChunks();
    if (savedChunks.length > 0) {
        chunks = savedChunks;
        receivedLength = chunks.reduce((sum, chunk) => sum + chunk.byteLength, 0);
        console.log('恢复下载进度');
    }

    // 开始下载
    while (receivedLength < fileSize) {
        const rangeStart = receivedLength;
        const rangeEnd = Math.min(receivedLength + chunkSize - 1, fileSize - 1);
        const chunk = await downloadChunk(rangeStart, rangeEnd);
        chunks.push(chunk);
        receivedLength += chunk.byteLength;
        console.log(`已下载：${((receivedLength / fileSize) * 100).toFixed(2)}%`);

        // 保存块到 IndexedDB
        saveChunk(chunk);
    }

    // 下载完成，清除已保存的块并保存文件
    clearChunks();
    const blob = new Blob(chunks, { type: 'application/zip' });
    saveAs(blob, 'largefile.zip');
}
```

#### 注意

- 使用 `IndexedDB` 可以更可靠地存储大量二进制数据。
- 由于异步操作较多，确保在数据库初始化完成后再开始下载。

## 总结

以上提供了完整的前后端代码示例，实现了通过 C# Web API 提供大文件的断点续传下载，并在前端使用 JavaScript 实现了下载和断点续传功能。

- **后端**：
  - 使用 ASP.NET Core Web API，实现了支持 `Range` 请求的文件下载接口。
  - 处理了请求头和响应头，确保客户端可以正确地进行范围请求。
- **前端**：
  - 使用 JavaScript 实现了文件的分块下载和断点续传。
  - 提供了使用 `localStorage` 和 `IndexedDB` 存储下载进度的示例。
  - 使用 `FileSaver.js` 库触发文件下载。

希望这些代码和说明能帮助您实现所需的功能。如有任何疑问，请随时提问。