namespace AuthorizationServer.Contracts;

public interface IDateTracking
{
    DateTimeOffset CreatedDate { get; set; }
    DateTimeOffset? LastModifiedDate { get; set; }
}