using ChargingStations.Domain.Enums;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Commands.CreateStation;

/// <summary>
/// Yeni istasyon oluşturma komutu.
/// Command'lar veri değiştiren işlemlerdir (Create, Update, Delete).
/// Query'lerden farkı: yan etkisi vardır (DB'ye yazma).
/// </summary>
public class CreateStationCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsOpen24Hours { get; set; } = true;
    public string? PhoneNumber { get; set; }

    /// <summary>İstasyon ile birlikte oluşturulacak soketler</summary>
    public List<CreateConnectorDto> Connectors { get; set; } = new();
}

/// <summary>
/// İstasyon oluşturulurken soket bilgisi.
/// </summary>
public class CreateConnectorDto
{
    public ConnectorType Type { get; set; }
    public double PowerKW { get; set; }
    public decimal? PricePerKWh { get; set; }
}
