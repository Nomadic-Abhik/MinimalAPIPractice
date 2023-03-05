using AutoMapper;
using MinimalAPI.Dto;
using MinimalAPI.Models;

namespace MinimalAPI.Mapper
{
    public class CommandMapper :Profile
    {
        public CommandMapper()
        {
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
          //  CreateMap< Command,CommandCreateDto > ();
            CreateMap<CommandUpdateDto, Command>();
           
        }   
           
    }
}
