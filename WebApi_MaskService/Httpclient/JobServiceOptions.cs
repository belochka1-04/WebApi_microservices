namespace WebApi_MaskService.Httpclient
{
    public class JobServiceOptions
    {
        public const string SectionName = "JobService";

        public string BaseUrl { get; set; } = string.Empty;
        public string GetJobDtoPath { get; set; } = "/api/Job/getdtojob/{id}";
    }

}
