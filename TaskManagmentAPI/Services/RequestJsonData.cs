using System.Runtime.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TaskManagmentAPI.Models;
using TaskManagmentAPI.Dtos;

namespace TaskManagmentAPI.Services
{
    [DataContract]

    public class RequestJsonData
    {
        public RequestJsonData()
        {
        }
        public RequestJsonData(List<TaskModel> data, string message, int status, string title)
        {
            Data = data;
            Message = message;
            Status = status;
            Title = title;
        }
        [DataMember]
        public List<TaskModel> Data { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public string Title { get; set; }
    }
}
