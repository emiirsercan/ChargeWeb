using ChargingStations.Application.Interfaces;
using ChargingStations.Persistence.Context;

namespace ChargingStations.Persistence;

/// <summary>
/// IUnitOfWork'ün EF Core implementasyonu.
///
/// ─── SENARYO: Banka Transferi Analojisi ────────────────────
/// Handler içinde şunu yapıyorsun:
///
///   // 1. İstasyon oluştur (henüz veritabanına yazılmadı — sadece bellekte)
///   await _stationRepository.AddAsync(station);
///
///   // 2. Connector'ları ekle (yine bellekte)
///   foreach (var connector in connectors)
///       await _connectorRepository.AddAsync(connector);
///
///   // 3. HEPSİNİ TEK SEFERDE veritabanına yaz!
///   await _unitOfWork.SaveChangesAsync();
///   //    ↑ Eğer bu başarısız olursa, hiçbir şey yazılmaz (rollback)
///
/// ─── EF CORE'UN CHANGE TRACKER'I ───────────────────────────
/// EF Core, DbContext üzerinden yapılan tüm entity işlemlerini
/// "Change Tracker" ile takip eder.
///
/// Add() → EntityState.Added    → INSERT SQL hazır
/// Update() → EntityState.Modified → UPDATE SQL hazır  
/// Remove() → EntityState.Deleted  → DELETE SQL hazır
///
/// SaveChangesAsync() çağrılınca → Tüm bu SQL'ler tek transaction'da çalışır
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Change Tracker'daki tüm değişiklikleri veritabanına yazar.
    /// Dönen int = etkilenen satır sayısı.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// DbContext'in bağlantısını ve kaynaklarını serbest bırakır.
    /// using {} bloğu veya DI sistemi otomatik çağırır.
    /// </summary>
    public void Dispose()
    {
        _context.Dispose();
    }
}
