namespace STA.TouristCareCRM.Plugins.Resources
{
    public enum PluginStages : int
    {
        PreValidation = 10,
        PreOperation = 20,
        MainOperation = 30,
        PostOperation = 40
    }

    public enum CustomerTypeCode : int
    {
        Anonymous = 0,
        Customer = 1
    }

    public enum CaseTypeCode : int
    {
        Enquiry = 1,
        Complaint = 2,
        Suggestion = 3,
        OutofScope = 4,
        Emergency = 5
    }

    public enum PendingWith : int
    {
        STAAgent = 1,
        MTLicensing = 2,
        STAPartnerships = 3,
        STAMediaEnquiries = 4,
        STAMarketing = 5,
        STALeadershipTeam = 6,
        MTTechnical = 7,
        VisitSaudiComLead = 8,
        VisitSaudiAppLead = 9,
        Jawazat = 10,
        CCHI = 11,
        MinistryofTransport = 12,
        MinistryofCulture = 13,
        MOMRA = 14,
        MinistryofForeignAffairs_MOFA = 15,
        CivilAviationAuthority = 16
    }

    public enum OperationType : int
    {
        Create = 1,
        Resolve = 20,
        Assign = 13,
        Share = 14,
        BothAssignAndShare = 948
    }

    public enum RequestType : int
    {
        PauseSLA = 1,
        ChangeCaseCategory = 2,
        CancelCase = 3,
        ChangeCasePriority = 4,
        MomentofDelight = 5
    }

    public enum EntityTypeCode : int
    {
        Account = 1,
        Contact = 2,
        Case = 112,
        SystemUser = 8,
        Team = 9
    }

    public enum NotificationLanguage : int
    {
        Arabic = 1025,
        English = 1033
    }

    public enum AssignToOrShareTo : int
    {
        User = 8,
        Team = 9
    }

    public enum SLAInitiateFrom : int
    {
        CreatedOn = 1,
        SubmittedOn = 2
    }

    public enum CaseSLAStatus : int
    {
        InProgress = 1,
        InWarning = 2,
        WithinSLA = 3,
        SLABreached = 4,
        SLAPaused = 5
    }

    public enum ObjectTypeCode : int
    {
        Case = 112
    }

    public enum EntityStateCode : int
    {
        Active = 0,
        InActive = 1
    }

    public enum EntityStatusCode : int
    {
        Active = 1,
        InActive = 2
    }

    public enum ActivityEntityStateCode : int
    {
        Open = 0,
        Completed = 1,
        Canceled = 2,
        Scheduled = 3
    }

    public enum ActivityEntityStatusCode : int
    {
        Open = 1,
        Completed = 2,
        Canceled = 3,
        Scheduled = 4
    }

    public enum CaseEntityStateCode : int
    {
        Active = 0,
        Resolved = 1,
        Cancelled = 2
    }

    public enum CaseEntityStatusCode : int
    {
        New = 1,
        UnderReview = 2,
        InProgress = 3,
        SLAPaused = 4,
        Resolved = 5,
        InformationProvided = 1000,
        FirstContactResolution = 948010000,
        Cancelled = 6,
        Merged = 2000
    }

    public enum RatingsType : int
    {
        CSAT_Agent = 1,
        CSAT_Information = 2,
        CSAT_Outcome = 3,
        NPS = 4,
        CES = 5
    }

    public enum Frequency : int
    {
        Monthly = 1,
        Quarterly = 2,
        Yearly = 3
    }
}