namespace TC.WrapperService.Resources
{
    public enum PluginStages
    {
        PreValidation = 10,
        PreOperation = 20,
        MainOperation = 30,
        PostOperation = 40
    }

    public enum PendingWith
    {
        MoTTechnicalTeam = 1,
        MoTLicensingTeam = 2,
        STAAgent = 3
    }

    public enum AssignTo
    {
        User = 1,
        Team = 2,
        Queue = 3
    }

    public enum SLAInitiateFrom
    {
        CreatedOn = 1,
        SubmittedOn = 2
    }

    public enum CaseSLAStatus
    {
        InProgress = 1,
        InWarning = 2,
        WithinSLA = 3,
        SLABreached = 4,
        SLAPaused = 5
    }

    public enum ObjectTypeCode
    {
        Case = 112
    }

    public enum EntityStateCode
    {
        Active = 0,
        InActive = 1
    }

    public enum EntityStatusCode
    {
        Active = 1,
        InActive = 2
    }

    public enum ChannelOrigin
    {
        PhoneCall = 1,
        Email = 2,
        Web = 3,
        Facebook = 4,
        Twitter = 5,
        Instagram = 6,
        Youtube = 7,
        WhatsApp = 8,
        LiveChat = 9,
        Kiosk = 10,
        BayenaPortal = 11,
        TripAdvisor = 12,
        MobileApp = 13,
        HelpVisitSaudi = 14,
        STAWeb = 17,
        VideoCall = 18,
        ChatBot = 19
    }

    public enum LeadType
    {
        International = 1,
        Domestic = 2
    }

    public enum BusinessType
    {
        Commercial = 948120000,
        TouristCare = 948120001,
        Both = 948120002,
    }

    public enum APIOperationType
    {
        Create = 1,
        Update = 2,
        Retrieve = 3,
        RetrieveMultiple = 4
    }

    public enum ArticleStatus 
    {
        Published = 3,
        Draft = 1,
    }

    public enum SSIDInterest 
    {
        SunAndSea = 0,
        Adventure = 1,
        Wellness = 2,
        CultureAndHeritage = 3,
        Culinary = 4,
        Entertainment = 5,
        Sports = 6,
        Cruise = 7,
        Shopping = 8
    }
}