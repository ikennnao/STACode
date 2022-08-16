using System;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{

    public class CreateConversationRequest
    {
        public CreateConversation createConversation { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class ConversationResponse
    {
        public string Subject { get; set; }
        public Guid ConversationGuid { get; set; }
        public string EntityLogicalName { get; set; }
    }

    public class CreateConversation
    {
        public string Subject { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string IncomingMessage { get; set; }
        public string MobileNumber { get; set; }
        public string ChannelOrigin { get; set; }
        public Guid ConversationGuid { get; set; }
        public string CaseRefNo { get; set; }
        public CustomEntityReference CaseType { get; set; }
        public CustomEntityReference Customer { get; set; }
    }
}