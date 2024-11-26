﻿using AutoMapper;
using DeclarationManagement.Model;
using DeclarationManagement.Model.DTO;

namespace DeclarationManagement;

public class AutoMapperProfile : Profile
{
    //添加项目间的映射关系
    public AutoMapperProfile()
    {
        // ApplicationForm 和 ApplicationFormDTO 之间的映射
        CreateMap<ApplicationForm, ApplicationFormDTO>().ForMember(dest => dest.ApprovalRecords,
            opt => opt.MapFrom(src => src.ApprovalRecords));//定义ApprovalRecords的映射关系
        CreateMap<ApplicationFormDTO, ApplicationForm>();
        CreateMap<ApplicationFormAlterDTO, ApplicationForm>();
        CreateMap<ApplicationForm, ApplicationFormAlterDTO>();

        // User 和 UserDTO 之间的映射
        CreateMap<User, UserDTO>();
        CreateMap<UserDTO, User>();

        // ApprovalRecord 和 ApprovalRecordDTO 之间的映射
        CreateMap<ApprovalRecord, ApprovalRecordDTO>();
        CreateMap<ApprovalRecordDTO, ApprovalRecord>();

        // TableSummary 和 TableSummaryDTO 之间的映射
        CreateMap<TableSummary, TableSummaryDTO>();
        CreateMap<TableSummaryDTO, TableSummary>();
    }
}