## 3.概念设计

### 3.1 实体关系图

### 3.2 实体定义

## 4. 逻辑模型设计

### 4.1 数据库概念

- 数据库命名：

### 4.2 表结构设计

- 表：User（用户）

  - 一个用户对应多个申请表单
  - 一个用户对应多个审批表汇总

  | 字段名    | 数据类型           | 允许为空 | 主键 | 外键 | 默认值 | 说明                               |
  | --------- | ------------------ | -------- | ---- | ---- | ------ | ---------------------------------- |
  | UserID    | INT AUTO_INCREMENT | 否       | 是   |      |        | 用户ID                             |
  | Username  | VARCHAR(50)        | 否       | \    |      |        | 用户名称                           |
  | Password  | VARCHAR(255)       | 否       | \    |      |        | 密码（应使用哈希存储）             |
  | Role      | VARCHAR(50)        | 否       | \    |      |        | 角色（理学院/教务处）              |
  | Power     | VARCHAR(50)        | 否       | \    |      |        | 权限（普通用户/预审用户/初审用户） |
  | JobNumber | VARCHAR(50)        | 否       | \    |      |        | 工号                               |
  | Name      | VARCHAR(50)        | 否       | \    |      |        | 姓名                               |

- 表：ApplicationForm（申请表单）

  - 一个申请表单对应多个审批记录表
  - 最终显示部分由终审人填写（若终审为拟不通过则打回修改）

  | 字段名             | 数据类型           | 允许为空 | 主键 | 外键             | 默认值 | 说明                                                         |
  | ------------------ | ------------------ | -------- | ---- | ---------------- | ------ | ------------------------------------------------------------ |
  | ApplicationFormID  | INT AUTO_INCREMENT | 否       | 是   |                  |        | 表单ID                                                       |
  | ProjectLeader      | VARCHAR(50)        | 否       | \    |                  |        | 项目负责人                                                   |
  | ContactWay         | VARCHAR(50)        | 是       | \    |                  |        | 联系方式                                                     |
  | Department         | VARCHAR(50)        | 是       | \    |                  |        | 所属部门                                                     |
  | ProjectName        | VARCHAR(150)       | 否       | \    |                  |        | 项目名称                                                     |
  | ProjectCategory    | VARCHAR(50)        | 否       | \    |                  |        | 项目类别                                                     |
  | ProjectLevel       | VARCHAR(50)        | 否       | \    |                  |        | 项目等级                                                     |
  | AwardLevel         | VARCHAR(50)        | 否       | \    |                  |        | 奖项级别                                                     |
  | ParticipationForm  | VARCHAR(50)        | 否       | \    |                  |        | 参与形式                                                     |
  | ApprovalFileName   | VARCHAR(500)       | 否       | \    |                  |        | 认定批文文件名称                                             |
  | ApprovalFileNumber | VARCHAR(500)       | 否       | \    |                  |        | 认定批文文件号                                               |
  | ItemDescription    | VARCHAR(8000)      | 否       | \    |                  |        | 项目内容                                                     |
  | ProjectOutcome     | VARCHAR(8000)      | 否       | \    |                  |        | 项目成果                                                     |
  | Decision           | INT                | 否       | \    |                  | 0      | 处理意见（0未审核，1拟同意，2拟不同意，3不同意）（最终显示） |
  | AuditDepartment    | VARCHAR(50)        | 是       | \    |                  |        | 审核部门（最终显示）拟同意才显示（根据审核人的Role决定）     |
  | Comments           | VARCHAR(8000)      | 是       | \    |                  |        | 原因（最终显示）拟同意才显示                                 |
  | RecognitionLevel   | VARCHAR(50)        | 是       | \    |                  |        | 认定等级（最终显示）拟同意才显示                             |
  | DeemedAmount       | INT                | 是       | \    |                  |        | 认定金额（最终显示）拟同意才显示                             |
  | Remarks            | VARCHAR(8000)      | 是       | \    |                  |        | 备注（最终显示）选填                                         |
  | UserID             | INT                | 否       | \    | 关联User表UserID |        | 用户ID（申请人）                                             |
  | ApprovalDate       | SMALLDATETIME      | 否       | \    |                  |        | 申请时间（第一次自动赋值）                                   |
  | States             | INT                | 否       | \    |                  |        | 审核状态（0未审核，1预审已完成，2初审完成）完成代表通过或不通过。 |
  
- 表：ApprovalRecords（审批记录表）生成后不再改变

  | 字段名            | 数据类型           | 允许为空 | 主键 | 外键                                   | 默认值 | 说明                                             |
  | :---------------- | ------------------ | -------- | ---- | -------------------------------------- | ------ | ------------------------------------------------ |
  | ApprovalID        | INT AUTO_INCREMENT | 否       | 是   |                                        |        | 审批记录ID                                       |
  | ApplicationFormID | INT                | 否       | \    | 关联ApplicationForm表ApplicationFormID |        | 申请表单ID（关联到表单）                         |
  | UserID            | INT                | 否       | \    | 关联User表UserID                       |        | 审批人ID（审批人ID最终选用Role）                 |
  | ApprovalDate      | SMALLDATETIME      | 否       | \    |                                        |        | 申请时间（审批时间）                             |
  | Decision          | INT                | 否       | \    |                                        |        | 审批决定（0未审核，1拟同意，2拟不同意，3不同意） |
  | Comments          | VARCHAR(8000)      | 是       | \    |                                        |        | 审批意见（拒绝或者同意的意见）                   |

- 表：审批表汇总（TableSummary）

  | 字段名            | 数据类型           | 允许为空 | 主键 | 外键                                   | 默认值 | 说明                                               |
  | ----------------- | ------------------ | -------- | ---- | -------------------------------------- | ------ | -------------------------------------------------- |
  | SummaryID         | INT AUTO_INCREMENT | 否       | 是   |                                        |        | 表汇总ID                                           |
  | UserID            | INT                | 否       |      | 关联User表UserID                       |        | 当前用户                                           |
  | ApplicationFormID | INT                | 否       |      | 关联ApplicationForm表ApplicationFormID |        | 表ID                                               |
  | Decision          | INT                | 否       |      |                                        |        | 是否已操作（0未审核，1拟同意，2拟不同意，3不同意） |

流程

- 用户登录

- 点击表单按钮

  - 普通用户：

    - 涉及事件

      - [x] 首次申请表单

        - 传入表单信息
      
        - 创建表单
        - 推送给符合的预审用户的审批表汇总（Role相等 && Powe等于预审用户）
          - 审批表汇总Decision为0
          - User关联被审批用户
          - ApplicationFormID 关联当前表单。
      
      - [x] 修改表单
      
        - 传入表单信息
        - 修改表单（Decision=0，state=0）
        - 推送给符合的预审用户的审批表汇总（Role相等 && Powe等于预审用户）（新建审批表汇总）
          - 审批表汇总Decision为0
          - User关联被审批用户
          - ApplicationFormID 关联当前表单。
      
      - [x] 查看表单
      
        - 返回当前表单
      
      - [x] 获取当前用户所有表单信息返回（将表单需要展示的显示）
      
        - 查找当前表单审批记录的
          - Decision = 1 （审核完成，查看）
          - Decision = 3 （审批不通过，查看）
          - Decision = 2 （审批驳回，修改）
          - Decision = 0 （待审核，查看）

    在（申请表单）中查询当前用户申请的所有表单（以ID从小到大排列） —— （查）

  - 审批用户：

    - 涉及事件
    
      - 判断当前审核用户
        - 根据User.Power/User.Role/ApplicationForm.ProjectCategory（项目类别来进行判断）
  
      - 审核表单
  
        - 预审用户
        - 填写表单数据
          - 修改当前审批汇总表
        - 创建新的审批记录表
            - ApplicationFormID 关联当前表单。
            - User关联被审批用户
          - Decision = 2 
            - 打回给普通用户（将ApplicationForm的表单中Decision改为2）
          - Decision = 1/3
            - 推送给符合的预审用户的审批表汇总（Role相等 && Powe等于预审用户）（新建审批表汇总）
              - 审批表汇总Decision为0
              - User关联相关初审用户
              - ApplicationFormID 关联当前表单。
      - 终身用户
          - 修改当前审批汇总表
        - 创建新的审批记录表
            - ApplicationFormID 关联当前表单。
          - User关联被审批用户
          - 判断
            - Decision = 2 
              - 打回给普通用户（将ApplicationForm的表单中Decision改为2）
            - Decision = 1/3 结束审批流程
          - 修改表单数据
    
      - 查看审批表汇总 （将表单需要展示的显示）
    
        - Decision = 1 （审核完成，查看）
        - Decision = 3 （审批不通过，查看）
        - Decision = 2 （审批驳回，查看）
        - Decision = 0 （待审核，审核）
    
        
  
    （审批表汇总）中查询当前用户所涉及的所有表单（以ID从小到大排列）——（查）





说明：

User用户分为：普通用户、预审用户、初审用户

普通用户只有项目申报页面

预审用户/初审用户只有项目审核页面





普通用户只有添加和修改表格的权限：

- Post请求：/addForm （添加表格）

  ```json
  {
    "applicationFormID": 0,
    "projectLeader": "王晨以",
    "contactWay": "182222132312",
    "department": "轨道交通学院",
    "projectName": "数理XXXXXX1",
    "projectCategory": "专业建设类",
    "projectLevel": "国家级",
    "awardLevel": "一等第",
    "participationForm": "个人",
    "approvalFileName": "文件11111",
    "approvalFileNumber": "123124214",
    "itemDescription": "XXXXXXXXX",
    "projectOutcome": "YYYYYYYYYY",
    "decision": 0,
    "auditDepartment": "string",
    "comments": "string",
    "recognitionLevel": "string",
    "deemedAmount": 0,
    "remarks": "string",
    "userID": 45,
    "approvalDate": "2024-09-30T08:46:18.063Z",
    "states": 0,
    "approvalRecords": []
  }
  ```
  
  C#承接类
  
  ```c#
  namespace DeclarationManagement.Model.DTO;
  
  public class ApplicationFormDTO
  {
      /// <summary>
      /// 表单ID（自动生成）
      /// </summary>
      public int ApplicationFormID { get; set; }
  
      /// <summary>
      /// 项目负责人（可修改）
      /// </summary>
      public string ProjectLeader { get; set; }
  
      /// <summary>
      /// 联系方式（可修改）
      /// </summary>
      public string ContactWay { get; set; }
  
      /// <summary>
      /// 所属部门（可修改）
      /// </summary>
      public string Department { get; set; }
  
      /// <summary>
      /// 项目名称（可修改）
      /// </summary>
      public string ProjectName { get; set; }
  
      /// <summary>
      /// 项目类别（可修改）
      /// </summary>
      public string ProjectCategory { get; set; }
  
      /// <summary>
      /// 项目等级（可修改）
      /// </summary>
      public string ProjectLevel { get; set; }
  
      /// <summary>
      /// 奖项级别（可修改）
      /// </summary>
      public string AwardLevel { get; set; }
  
      /// <summary>
      /// 参与形式（可修改）
      /// </summary>
      public string ParticipationForm { get; set; }
  
      /// <summary>
      /// 认定批文文件名称（可修改）
      /// </summary>
      public string ApprovalFileName { get; set; }
  
      /// <summary>
      /// 认定批文文件号（可修改）
      /// </summary>
      public string ApprovalFileNumber { get; set; }
  
      /// <summary>
      /// 项目内容（可修改）
      /// </summary>
      public string ItemDescription { get; set; }
  
      /// <summary>
      /// 项目成果（可修改）
      /// </summary>
      public string ProjectOutcome { get; set; }
  
      /// <summary>
      /// 最终处理意见（可修改，（0未审核，1拟同意，2拟不同意，3不同意））
      /// 默认值为0
      /// </summary>
      public int Decision { get; set; }
  
      /// <summary>
      /// 审核部门（最终输入）
      /// </summary>
      public string AuditDepartment { get; set; }
  
      /// <summary>
      /// 原因（最终输入）
      /// </summary>
      public string Comments { get; set; }
  
      /// <summary>
      /// 认定等级（最终输入）
      /// </summary>
      public string RecognitionLevel { get; set; }
  
      /// <summary>
      /// 认定金额（最终输入）
      /// </summary>
      public decimal DeemedAmount { get; set; }
  
      /// <summary>
      /// 备注（最终输入）
      /// </summary>
      public string Remarks { get; set; }
  
      /// <summary>
      /// 用户ID（关联到User表中）
      /// </summary>
      public int UserID { get; set; }
  
  
      /// <summary>
      /// 申请时间（一次记录）
      /// </summary>
      public DateTime ApprovalDate { get; set; }
  
      //审批记录表中不放这个部分
      public List<ApprovalRecordDTO> ApprovalRecords { get; set; } = new List<ApprovalRecordDTO>(); //这个也需要对DTO做映射处理
  }
  ```
  
  ```c#
  /// <summary> 审核表单DTO </summary>
  public class ApprovalRecordDTO
  {
      public int ApprovalRecordID { get; set; }
      public int ApplicationFormID { get; set; }
      public int UserID { get; set; }
      public DateTime ApprovalDate { get; set; }//时间
      public int Decision { get; set; }//当前审批决定
      public string Comments { get; set; }//审批原因
  }
  ```
  
  
  
- Put请求：/alterForm （修改表单）

  ```json
  {
    "applicationFormID": 0,
    "projectLeader": "王晨",
    "contactWay": "18200001111",
    "department": "数理学院",
    "projectName": "数理XXXXXX1",
    "projectCategory": "专业建设类",
    "projectLevel": "国家级",
    "awardLevel": "一等第",
    "participationForm": "个人",
    "approvalFileName": "文件11111",
    "approvalFileNumber": "100000000000X",
    "itemDescription": "XXXXXXXXX",
    "projectOutcome": "YYYYYYYYYY",
    "decision": 0,
    "auditDepartment": "string",
    "comments": "string",
    "recognitionLevel": "string",
    "deemedAmount": 0,
    "remarks": "string",
    "userID": 1,
    "approvalDate": "2024-09-30T08:46:18.063Z",
    "states": 0,
    "approvalRecords": [
      {
        "approvalRecordID": 0,
        "applicationFormID": 0,
        "userID": 0,
        "approvalDate": "2024-10-08T15:17:07.898Z",
        "decision": 0,
        "comments": "string"
      }//若没有则传送空列表
    ]
  }
  ```
  
  C#承接类如上

预审用户和初审用户只有审批权限：（并且初审用户需要填写部分表单信息，这些信息只有初审用户填写）

- Post请求：/approvalForm （审批表单）

  ```json
  {
    "applicationFormID": 0,
    "tableSummaryID": 0,
    "userID": 0,
    "decision": 0,
    "comments": "string",
    "recognitionLevel": "string",
    "deemedAmount": 0,
    "remarks": "string"
  }
  ```

  C#对应字段

  ```c#
  namespace DeclarationManagement.Model.DTO;
  
  /// <summary> 审核组合代码 </summary>
  public class ApprovalCombineDTO
  {
      /// <summary>
      /// 所审核的表单ID
      /// </summary>
      public int applicationFormID { get; set; }
      
      /// <summary>
      /// 当前的审批用户的表汇总ID
      /// </summary>
      public int TableSummaryID { get; set; }
  
      #region 审批记录部分
  
      /// <summary>
      /// 审批人ID
      /// </summary>
      public int UserID { get; set; }
  
      /// <summary>
      /// 当前审批决定
      /// </summary>
      public int Decision { get; set; }
  
      /// <summary>
      /// 审批意见（原因）
      /// </summary>
      public string Comments { get; set; }
      
      /// <summary>
      /// 认定等级
      /// </summary>
      public string RecognitionLevel { get; set; }
      
      
      /// <summary>
      /// 认定金额
      /// </summary>
      public int DeemedAmount { get; set; }
      
      /// <summary>
      /// 备注（选填）
      /// </summary>
      public string Remarks { get; set; }
  
      #endregion
      
  }
  ```

  - 审核Decision （这个数据传入需要非0
    - Decision = 1 （审核完成，查看）
    - Decision = 3 （审批不通过，查看）
    - Decision = 2 （审批驳回，查看）
  - RecognitionLevel、DeemedAmount、Remarks需要初审用户填写。

通用请求：

- 显示的是当前用户的所有表单状态（普通用户就是所有的申请表单，初审/预审则是所有的审核表单状态）

  普通用户：formID相当于是ApplictionFormID，

  预审/初审用户：formID相当于是TableSummaryID，

  Get请求：/getUserStates/{UserID}

  - 返回数据

    ```json
      {
        "formID": 1,
        "projectLeader": "王晨",
        "department": "数理学院",
        "projectName": "数理XXXXXX1",
        "projectCategory": "专业建设类",
        "projectLevel": "国家级",
        "approvalDate": "2024-10-01T11:03:00",
        "decision": 1
      },
      {
        "formID": 2,
        "projectLeader": "陈彧",
        "department": "数理学院",
        "projectName": "数理XXXXXX1",
        "projectCategory": "师资建设类",
        "projectLevel": "市级",
        "approvalDate": "2024-10-01T11:10:00",
        "decision": 1
      }
    ```

    对应C#字段

    ```c#
    public class CommonDatas
    {
        /// <summary>
        /// 表单ID 普通用户时ApplicationForm，，初审和终审则是TableSummary
        /// </summary>
        public int FormID { get; set; } //表单ID
    
        /// <summary>
        /// 项目负责人（可修改）
        /// </summary>
        public string ProjectLeader { get; set; }
    
        /// <summary>
        /// 所属部门（可修改）
        /// </summary>
        public string Department { get; set; }
    
        /// <summary>
        /// 项目名称（可修改）
        /// </summary>
        public string ProjectName { get; set; }
    
        /// <summary>
        /// 项目类别（可修改）
        /// </summary>
        public string ProjectCategory { get; set; }
    
        /// <summary>
        /// 项目等级（可修改）
        /// </summary>
        public string ProjectLevel { get; set; }
    
        /// <summary>
        /// 申请时间（一次记录）
        /// </summary>
        public DateTime ApprovalDate { get; set; }
    
        /// <summary>
        /// 审批决定（0未审核，1拟同意，2拟不同意，3不同意）
        /// </summary>
        public int Decision { get; set; }
    }
    ```

- 登录请求：

  Post请求：/api/Public/login  登录请求

  ```json
  {
    "username": "string",
    "password": "string"
  }
  ```

  账户密码验证正确后，返回下方Json。否则返回账号或密码不正确。
  
  ```c#
  {
    "userID": 1,
    "userPower": "普通用户",
    "commonDatasModel": [
      {
        "formID": 1,
        "projectLeader": "王晨",
        "department": "数理学院",
        "projectName": "数理XXXXXX1",
        "projectCategory": "专业建设类",
        "projectLevel": "国家级",
        "approvalDate": "2024-10-01T11:03:00",
        "decision": 0
      },
      {
        "formID": 3,
        "projectLeader": "王晨琛",
        "department": "数理学院",
        "projectName": "数理XXXXXX2",
        "projectCategory": "专业建设类",
        "projectLevel": "国家级",
        "approvalDate": "2024-10-08T22:19:00",
        "decision": 0
      },
      {
        "formID": 4,
        "projectLeader": "王晨以",
        "department": "数理学院",
        "projectName": "数理X1111",
        "projectCategory": "专业建设类",
        "projectLevel": "国家级",
        "approvalDate": "2024-10-08T22:22:00",
        "decision": 0
      }
    ]
  }
  ```
  
  //UserID和UserPower外，其余都和上方一致
  
  


- 获取单个表单

  "/getForm/{getCode}/{FormId}"

  > 这个FormID就是commonDatasModel中的FormID

  getCode=0 则是直接查看表单 getCode=1 则是审核界面查看表单

  返回值
  
  ```c#
  {
    "applicationFormID": 1,
    "projectLeader": "王晨",
    "contactWay": "18200001111",
    "department": "数理学院",
    "projectName": "数理XXXXXX1",
    "projectCategory": "专业建设类",
    "projectLevel": "国家级",
    "awardLevel": "一等第",
    "participationForm": "个人",
    "approvalFileName": "文件11111",
    "approvalFileNumber": "100000000000X",
    "itemDescription": "XXXXXXXXX",
    "projectOutcome": "YYYYYYYYYY",
    "decision": 0,
    "auditDepartment": "string",
    "comments": "string",
    "recognitionLevel": "string",
    "deemedAmount": 0,
    "remarks": "string",
    "userID": 1,
    "approvalDate": "2024-10-01T11:03:00",
    "approvalRecords": [
      {
        "approvalRecordID": 18,
        "applicationFormID": 1,
        "userID": 3,
        "approvalDate": "2024-10-08T23:42:00",
        "decision": 1,
        "comments": "string"
      }
    ]
  }
  ```

  
  
  
  
  
  
  # 修改部分：
  
  - [x] UserRole不作为判断依据：
  
    流转体系将UserRole改为Department；
    
  - [ ] Bug功能查找，报错部分
  
  
  
  
  
  # 新增功能：
  
  - [ ] Excel导出功能
  - [x] 修改密码的功能
  - [x] 教务处可以查看所有已经完成的审核
    - [ ] 前端页面代做
  - [ ] 申请列表时，Department变为选项（默认从UserRole中获取）
  
  
  
  # 讨论：
  
  - [ ] 添加表单成功后是否需要什么返回值
