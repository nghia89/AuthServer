

namespace AuthorizationServer.Contracts;


public interface IEntityBase<T>
{
    T Id { get; set; }
}