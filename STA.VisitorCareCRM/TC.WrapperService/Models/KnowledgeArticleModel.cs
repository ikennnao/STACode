using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{
    public class RetrieveKnowledgeRequest
    {
        public RetrieveKnowledgeArticleObj retrieveKnowledgeArticle { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class RetrieveKnowledgeArticleObj
    {
        public string Keywords { get; set; }
        public string Status { get; set; }
        public bool IsContentRequired { get; set; }
        public string IsInternalArticle { get; set; }
        public Guid KnowledgeArticleGuid { get; set; }

        public string CategoryCode { get; set; }
        public string LanguageCode { get; set; }
    }

    public class RetrieveKnowledgeArticle
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string AppRecordUrl { get; set; }
        public CustomOptionsetValue Status { get; set; }
        public Guid KnowledgeArticleGuid { get; set; }
        public string KeyWords { get; set; }
        public int? Order { get; set; }
    }

    public class RetrieveKnowledgeResponse
    {
        public string knowledgeArticleETN { get; set; }
        public List<RetrieveKnowledgeArticle> kaDataObj { get; set; }
    }
}