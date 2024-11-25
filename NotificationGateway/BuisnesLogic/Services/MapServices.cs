using AutoMapper;
using BuisnesLogic.Extensions;
using BuisnesLogic.Models.Kafka;
using BuisnesLogic.Models.Messages;
using BuisnesLogic.Models.Requests;
using BuisnesLogic.Models.Requests.Auth;
using BuisnesLogic.Models;
using BuisnesLogic.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Abstractions;
using Data.Models;
namespace BuisnesLogic.Realization
{
    public delegate void Set<T,Y>(T request, Y transportModel) where T : Request where Y : MessageTransportContract;
    public class MapServices
    {
        public static IList<KafkaFileTransport> ParseFileToModel(IEnumerable<IFormFile>? files, bool isAttachment)
        {
            var kafkaFileTransports = new List<KafkaFileTransport>();
            if (files is not null)
            {
                foreach (var item in files)
                {
                    using var stream = item.OpenReadStream();
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    kafkaFileTransports.Add(new KafkaFileTransport(data, item.FileName, isAttachment));
                }
            }
            return kafkaFileTransports;

        }
        public ServiceAuthModel MapFromDbAuthModel(AuthRequest model)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<AuthRequest, ServiceAuthModel>()
               .ForMember("Password", opt => opt.MapFrom(o => o.Password))
               .ForMember("Login", opt => opt.MapFrom(o => o.Login))
               ));
            var newModel = mapper.Map<ServiceAuthModel>(model);
            var passwordInfo = newModel.Password.EncryptPassword(28);
            newModel.Password = passwordInfo.passwordhash;
            newModel.SaltForPassword = passwordInfo.salt;
            return newModel;
        }
        public StateMessageModel MapFromDbModel(Request model, MessageType messageType)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Request, StateMessageModel>()
               .ForMember("Subject", opt => opt.MapFrom(o => o.Subject))
               .ForMember("Content", opt => opt.MapFrom(o => o.Message))
               ));
            var messageInBase = mapper.Map<StateMessageModel>(model);
            List<KafkaFileTransport> files = new List<KafkaFileTransport>();
            if (model.FilesInAttachment is not null) files.AddRange(ParseFileToModel(model.FilesInAttachment, true)!);
            if (model.FilesInAttachment is not null) files.AddRange(ParseFileToModel(model.FilesInBody, false)!);
            messageInBase.Files = files.ConvertToBinaryFilesForDb();
            messageInBase.TimeSended = DateTime.UtcNow;
            messageInBase.State = State.Sended.ToString();
            messageInBase.MessageType = messageType.ToString();
            return messageInBase;
        }
        public Y MapFromKafkaModel<T,Y>(T request, Set<T,Y> set) where T : Request where Y : MessageTransportContract
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<T, Y>()
               .ForMember("Subject", opt => opt.MapFrom(o => o.Subject))
               .ForMember("Message", opt => opt.MapFrom(o => o.Message))
               ));
            var message = mapper.Map<Y>(request);
            List<KafkaFileTransport> Files =
            [
               .. ParseFileToModel(request.FilesInAttachment, true),
                ..ParseFileToModel(request.FilesInBody, false),
            ];
            message.Files = Files;
            set(request, message);
            return message;
        }
    }
}
