interface User {
  fullName: string;
  email: string;
  role: string;
}

interface ProfilePageProps {
  user: User | null;
}

export default function ProfilePage({ user }: ProfilePageProps) {
  return (
    <div className="max-w-max-width mx-auto px-margin-mobile md:px-margin-desktop py-8 md:py-12">
      {/* Profile Header Bento Grid Section */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-12">
        {/* User Identity Card */}
        <div className="md:col-span-2 glass-card p-8 rounded-3xl flex flex-col md:flex-row items-center md:items-start gap-8">
          <div className="relative">
            <div className="w-32 h-32 rounded-3xl overflow-hidden border-4 border-white shadow-lg bg-surface-container">
              <img 
                className="w-full h-full object-cover" 
                alt="Profile Avatar"
                src="https://lh3.googleusercontent.com/aida-public/AB6AXuD_cGjeStQSa1hUSAHwyI_EiHiBpChOUY9q7Z7rBFt1aErHWan7g8BPkIBSfYO9mOQ1c-ydvUX8ds-ZLjQtc3VOafuR6Sgh62Pfbi1hbTpK2Ts_cmB_MUGopnYFBk4Ft8K9gbNSIxLFMai-oXIfeVG4TNIKlhp32b_pClaex3R4wrU3pCcRpFe54K-dMepsU2sDGnJNr1fghv1PEUzpNFqvUPKnA1ZPkn9lxd5F8rRFGrXA7yCJpulGGDWa9sj2-GVyZcqHvT7AeCDf"
              />
            </div>
            <div className="absolute -bottom-2 -right-2 bg-secondary text-white px-3 py-1 rounded-full text-[11px] font-semibold flex items-center gap-1 shadow-md">
              <span className="material-symbols-outlined text-[14px]" style={{ fontVariationSettings: "'FILL' 1" }}>star</span>
              Altın Üye
            </div>
          </div>
          <div className="flex flex-col text-center md:text-left justify-center h-full">
            <h1 className="font-headline-lg text-headline-lg text-on-surface font-bold mb-1">{user?.fullName || 'Ahmet Yılmaz'}</h1>
            <p className="font-body-md text-body-md text-on-surface-variant mb-4">Üyelik Tarihi: Mart 2023</p>
            <div className="flex flex-wrap justify-center md:justify-start gap-3">
              <span className="bg-primary-container/10 text-primary px-4 py-2 rounded-xl text-label-md font-label-md flex items-center gap-2">
                <span className="material-symbols-outlined text-[18px]">bolt</span>
                420 kWh Tüketim
              </span>
              <span className="bg-secondary-container/10 text-secondary px-4 py-2 rounded-xl text-label-md font-label-md flex items-center gap-2">
                <span className="material-symbols-outlined text-[18px]">eco</span>
                120kg CO2 Tasarrufu
              </span>
            </div>
          </div>
        </div>

        {/* Quick Action Stats */}
        <div className="bg-primary text-white p-8 rounded-3xl flex flex-col justify-between shadow-xl">
          <div>
            <h3 className="font-label-md text-label-md opacity-80 mb-2 uppercase tracking-wider">Cüzdan Bakiyesi</h3>
            <div className="flex items-baseline gap-2">
              <span className="font-display-lg text-display-lg font-bold">₺1.240</span>
              <span className="font-headline-md text-headline-md opacity-80 font-bold">.50</span>
            </div>
          </div>
          <button className="mt-8 bg-white text-primary w-full py-4 rounded-2xl font-label-md text-label-md font-bold flex items-center justify-center gap-2 hover:scale-[0.98] transition-transform active:opacity-90">
            <span className="material-symbols-outlined">add_circle</span>
            Bakiye Yükle
          </button>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Left Column: Vehicle & Settings */}
        <div className="lg:col-span-1 space-y-8">
          {/* My Vehicle Section */}
          <section>
            <h2 className="font-headline-md text-headline-md font-bold mb-4 px-2">Aracım</h2>
            <div className="bg-white rounded-3xl p-6 shadow-sm border border-outline-variant/20 overflow-hidden relative">
              <div className="absolute top-4 right-4">
                <span className="material-symbols-outlined text-primary cursor-pointer hover:rotate-12 transition-transform">settings</span>
              </div>
              <div className="mb-4">
                <h3 className="font-headline-md text-headline-md font-bold text-on-surface">Tesla Model Y</h3>
                <p className="font-body-md text-body-md text-on-surface-variant">34 ABC 123</p>
              </div>
              <div className="aspect-video rounded-2xl overflow-hidden mb-6 bg-surface-container">
                <img 
                  className="w-full h-full object-cover" 
                  alt="Vehicle Image"
                  src="https://lh3.googleusercontent.com/aida-public/AB6AXuDGw8PZXxo1XTqPGm5_lY0Ll9BLg3EXUdnA27-1hxxCP3hJDXwfPU3PfY4Wl4iuRRlSsS-Jc1w7-uFcgpe0ljOE0EsdzyAoztGxSdv3il2yA6IEy0htTKYNlM5FnBeDQDtCV2pbcig-pbHDdIIugUhTBU7cUAYI7TGnttNUh1S6FaXk8hYKgJsRQEgS7MpnGhZvNvPxmV4PDITBp3CHxVYmdgpM5gdLZUojtcAG1DX_SIEkhuEnfl--b9Vvasy7YIsyPNoaokLSSckU"
                />
              </div>
              <div className="space-y-4">
                <div className="flex justify-between items-end mb-1">
                  <span className="font-label-md text-label-md text-on-surface-variant font-medium">Batarya Durumu</span>
                  <span className="font-headline-md text-headline-md text-secondary font-bold">%78</span>
                </div>
                <div className="w-full h-3 bg-surface-container rounded-full overflow-hidden">
                  <div className="h-full bg-secondary w-[78%] charging-progress-glow rounded-full"></div>
                </div>
                <div className="flex justify-between font-label-sm text-label-sm text-on-surface-variant pt-2">
                  <div className="flex items-center gap-1">
                    <span className="material-symbols-outlined text-[16px]">distance</span>
                    342 km menzil
                  </div>
                  <div className="flex items-center gap-1 text-secondary">
                    <span className="material-symbols-outlined text-[16px]">bolt</span>
                    Sağlıklı
                  </div>
                </div>
              </div>
            </div>
          </section>

          {/* Account Settings */}
          <section>
            <h2 className="font-headline-md text-headline-md font-bold mb-4 px-2">Hesap Ayarları</h2>
            <div className="bg-white rounded-3xl p-2 shadow-sm border border-outline-variant/20">
              <a className="flex items-center justify-between p-4 rounded-2xl hover:bg-surface-variant/30 transition-colors group" href="#">
                <div className="flex items-center gap-4">
                  <div className="w-10 h-10 rounded-xl bg-primary-container/10 text-primary flex items-center justify-center">
                    <span className="material-symbols-outlined">person_edit</span>
                  </div>
                  <span className="font-label-md text-label-md font-semibold">Profili Düzenle</span>
                </div>
                <span className="material-symbols-outlined text-outline group-hover:translate-x-1 transition-transform">chevron_right</span>
              </a>
              <a className="flex items-center justify-between p-4 rounded-2xl hover:bg-surface-variant/30 transition-colors group" href="#">
                <div className="flex items-center gap-4">
                  <div className="w-10 h-10 rounded-xl bg-primary-container/10 text-primary flex items-center justify-center">
                    <span className="material-symbols-outlined">notifications_active</span>
                  </div>
                  <span className="font-label-md text-label-md font-semibold">Bildirim Tercihleri</span>
                </div>
                <span className="material-symbols-outlined text-outline group-hover:translate-x-1 transition-transform">chevron_right</span>
              </a>
              <a className="flex items-center justify-between p-4 rounded-2xl hover:bg-surface-variant/30 transition-colors group" href="#">
                <div className="flex items-center gap-4">
                  <div className="w-10 h-10 rounded-xl bg-primary-container/10 text-primary flex items-center justify-center">
                    <span className="material-symbols-outlined">security</span>
                  </div>
                  <span className="font-label-md text-label-md font-semibold">Güvenlik ve Şifre</span>
                </div>
                <span className="material-symbols-outlined text-outline group-hover:translate-x-1 transition-transform">chevron_right</span>
              </a>
            </div>
          </section>
        </div>

        {/* Right Column: History & Payment */}
        <div className="lg:col-span-2 space-y-8">
          {/* Charging History */}
          <section>
            <div className="flex justify-between items-center mb-4 px-2">
              <h2 className="font-headline-md text-headline-md font-bold">Şarj Geçmişi</h2>
              <button className="text-primary font-label-md text-label-md font-semibold hover:underline">Tümünü Gör</button>
            </div>
            <div className="bg-white rounded-3xl shadow-sm border border-outline-variant/20 divide-y divide-outline-variant/20">
              {/* Session 1 */}
              <div className="p-6 flex flex-col md:flex-row md:items-center justify-between gap-4 hover:bg-surface-variant/10 transition-colors">
                <div className="flex gap-4">
                  <div className="w-12 h-12 rounded-2xl bg-secondary-container/20 text-secondary flex items-center justify-center shrink-0">
                    <span className="material-symbols-outlined" style={{ fontVariationSettings: "'FILL' 1" }}>ev_station</span>
                  </div>
                  <div>
                    <h4 className="font-label-md text-label-md font-bold text-on-surface">Zorlu Center - A Blok</h4>
                    <p className="font-label-sm text-label-sm text-on-surface-variant">12 Ekim 2023 • 14:20</p>
                  </div>
                </div>
                <div className="flex items-center justify-between md:justify-end gap-8 flex-1">
                  <div className="text-right">
                    <p className="font-label-md text-label-md font-bold text-on-surface">45.2 kWh</p>
                    <p className="font-label-sm text-label-sm text-on-surface-variant">DC Hızlı Şarj</p>
                  </div>
                  <div className="text-right min-w-[80px]">
                    <p className="font-label-md text-label-md font-bold text-primary">₺342.50</p>
                    <span className="text-[10px] uppercase font-bold tracking-tighter text-secondary bg-secondary/10 px-2 py-0.5 rounded">Tamamlandı</span>
                  </div>
                  <button className="p-2 text-outline hover:text-primary transition-colors">
                    <span className="material-symbols-outlined">receipt_long</span>
                  </button>
                </div>
              </div>

              {/* Session 2 */}
              <div className="p-6 flex flex-col md:flex-row md:items-center justify-between gap-4 hover:bg-surface-variant/10 transition-colors">
                <div className="flex gap-4">
                  <div className="w-12 h-12 rounded-2xl bg-secondary-container/20 text-secondary flex items-center justify-center shrink-0">
                    <span className="material-symbols-outlined" style={{ fontVariationSettings: "'FILL' 1" }}>ev_station</span>
                  </div>
                  <div>
                    <h4 className="font-label-md text-label-md font-bold text-on-surface">İstinye Park P2</h4>
                    <p className="font-label-sm text-label-sm text-on-surface-variant">08 Ekim 2023 • 18:45</p>
                  </div>
                </div>
                <div className="flex items-center justify-between md:justify-end gap-8 flex-1">
                  <div className="text-right">
                    <p className="font-label-md text-label-md font-bold text-on-surface">22.8 kWh</p>
                    <p className="font-label-sm text-label-sm text-on-surface-variant">AC Standart</p>
                  </div>
                  <div className="text-right min-w-[80px]">
                    <p className="font-label-md text-label-md font-bold text-primary">₺118.00</p>
                    <span className="text-[10px] uppercase font-bold tracking-tighter text-secondary bg-secondary/10 px-2 py-0.5 rounded">Tamamlandı</span>
                  </div>
                  <button className="p-2 text-outline hover:text-primary transition-colors">
                    <span className="material-symbols-outlined">receipt_long</span>
                  </button>
                </div>
              </div>

              {/* Session 3 */}
              <div className="p-6 flex flex-col md:flex-row md:items-center justify-between gap-4 hover:bg-surface-variant/10 transition-colors">
                <div className="flex gap-4">
                  <div className="w-12 h-12 rounded-2xl bg-secondary-container/20 text-secondary flex items-center justify-center shrink-0">
                    <span className="material-symbols-outlined" style={{ fontVariationSettings: "'FILL' 1" }}>ev_station</span>
                  </div>
                  <div>
                    <h4 className="font-label-md text-label-md font-bold text-on-surface">Emaar Square Mall</h4>
                    <p className="font-label-sm text-label-sm text-on-surface-variant">03 Ekim 2023 • 10:15</p>
                  </div>
                </div>
                <div className="flex items-center justify-between md:justify-end gap-8 flex-1">
                  <div className="text-right">
                    <p className="font-label-md text-label-md font-bold text-on-surface">38.5 kWh</p>
                    <p className="font-label-sm text-label-sm text-on-surface-variant">DC Hızlı Şarj</p>
                  </div>
                  <div className="text-right min-w-[80px]">
                    <p className="font-label-md text-label-md font-bold text-primary">₺285.40</p>
                    <span className="text-[10px] uppercase font-bold tracking-tighter text-secondary bg-secondary/10 px-2 py-0.5 rounded">Tamamlandı</span>
                  </div>
                  <button className="p-2 text-outline hover:text-primary transition-colors">
                    <span className="material-symbols-outlined">receipt_long</span>
                  </button>
                </div>
              </div>
            </div>
          </section>

          {/* Payment Methods */}
          <section>
            <div className="flex justify-between items-center mb-4 px-2">
              <h2 className="font-headline-md text-headline-md font-bold">Ödeme Yöntemleri</h2>
              <button className="text-primary font-label-md text-label-md flex items-center gap-1 font-bold hover:underline">
                <span className="material-symbols-outlined text-[18px]">add</span>
                Yeni Kart Ekle
              </button>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {/* Saved Card */}
              <div className="bg-white p-6 rounded-3xl border border-outline-variant/30 shadow-sm flex items-center justify-between">
                <div className="flex items-center gap-4">
                  <div className="w-12 h-8 bg-surface-container rounded flex items-center justify-center border border-outline-variant/20">
                    <img 
                      alt="Mastercard" 
                      className="h-4" 
                      src="https://lh3.googleusercontent.com/aida-public/AB6AXuCK0JLNCeRcvz5zTK61_vW15NSAFagSIx_YLQn-xRzYkxMzzBiiHHgKarq66QWKSfKI7lOnpvw48mZjjp6Yxv-q5DpmICELXbuhJNs_abdGK7sjbj4pZm11MG_KGydhmKhK0EqVmCrRsO91f4PXZUN-ZMqwEHihIv9VOTeA1apsLGBq4aziZ2Xsqou2IIfzt4-wSWf9N0EBd36LEfSrkit4TvYAWBfX1yuKrvLQzBnGe2zYZnqaO5Fd1oLP69ArHUXBYtyH59rGMGCw"
                    />
                  </div>
                  <div>
                    <p className="font-label-md text-label-md font-bold text-on-surface">{user?.fullName || 'Ahmet Yılmaz'}</p>
                    <p className="font-label-sm text-label-sm text-on-surface-variant">**** **** **** 4412</p>
                  </div>
                </div>
                <span className="bg-primary/10 text-primary text-[10px] font-bold px-2 py-1 rounded-full uppercase tracking-widest">Varsayılan</span>
              </div>
              {/* Secondary Card (Visual Balance) */}
              <div className="bg-surface-container-low p-6 rounded-3xl border border-dashed border-outline-variant/50 flex items-center justify-center gap-3 text-on-surface-variant hover:bg-surface-variant/30 cursor-pointer transition-colors">
                <span className="material-symbols-outlined">credit_card</span>
                <span className="font-label-md text-label-md">Yedek Ödeme Yöntemi Ekle</span>
              </div>
            </div>
          </section>
        </div>
      </div>
    </div>
  );
}
