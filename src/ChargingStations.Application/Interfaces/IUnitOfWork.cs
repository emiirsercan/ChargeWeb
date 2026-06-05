namespace ChargingStations.Application.Interfaces;

/// <summary>
/// Unit of Work pattern — birden fazla repository'deki değişikliği
/// tek bir transaction'da veritabanına yazmanı sağlar.
/// 
/// Örnek senaryo:
///   1. Yeni istasyon oluştur (StationRepository.Add)
///   2. İstasyona connector ekle (ConnectorRepository.Add)
///   3. Hepsini tek seferde kaydet → UnitOfWork.SaveChangesAsync()
///   
/// Eğer 2. adımda hata olursa, 1. adım da geri alınır (rollback).
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
