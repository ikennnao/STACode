namespace TC.WrapperService.Interfaces
{
    public interface IAdapter
    {
        string ConnectionString { get; set; }

        bool IsAdapterActive { get; set; }
    }
}