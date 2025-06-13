using System;

namespace vc.Ifx.Services;

public class ServiceBase : IService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}