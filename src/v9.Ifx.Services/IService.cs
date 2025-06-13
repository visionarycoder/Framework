using System;

namespace vc.Ifx.Services;

public interface IService
{
    public Guid InstanceId { get; }
}