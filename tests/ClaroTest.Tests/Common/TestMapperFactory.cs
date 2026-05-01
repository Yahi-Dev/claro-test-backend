using AutoMapper;
using ClaroTest.Core.Application.Mappings;

namespace ClaroTest.Tests.Common;

internal static class TestMapperFactory
{
    public static IMapper Create()
        => new MapperConfiguration(cfg => cfg.AddProfile<GeneralProfile>(), NullLoggerFactory.Instance)
            .CreateMapper();

    private static class NullLoggerFactory
    {
        public static Microsoft.Extensions.Logging.ILoggerFactory Instance { get; } =
            Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance;
    }
}
