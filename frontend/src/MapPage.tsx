import { useState, useEffect } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

// Import Leaflet assets explicitly to fix Vite asset path issues
import iconUrl from 'leaflet/dist/images/marker-icon.png';
import iconRetinaUrl from 'leaflet/dist/images/marker-icon-2x.png';
import shadowUrl from 'leaflet/dist/images/marker-shadow.png';

// Setup Leaflet icon defaults
const defaultIcon = L.icon({
  iconUrl,
  iconRetinaUrl,
  shadowUrl,
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41]
});

// Setup custom Green Icon for active/rapid chargers to match Stitch's design
const greenIcon = L.icon({
  iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-green.png',
  shadowUrl,
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41]
});

// Component to dynamically pan/zoom map when active station changes
function ChangeMapView({ center }: { center: [number, number] }) {
  const map = useMap();
  useEffect(() => {
    map.setView(center, 12, { animate: true });
  }, [center, map]);
  return null;
}

interface Station {
  ocmId: number;
  name: string;
  address: string;
  city: string;
  district: string;
  latitude: number;
  longitude: number;
  operatorName: string;
  statusText: string;
  connectorCount: number;
  distanceKm: number | null;
}

interface StationDetail {
  id: string;
  ocmId: number;
  name: string;
  address: string;
  city: string;
  district: string;
  latitude: number;
  longitude: number;
  operatorName: string;
  statusText: string;
  averageRating: number;
  reviewCount: number;
  connectors: Array<{
    connectionType: string;
    powerKW: number | null;
    currentType: string;
    quantity: number | null;
    status: string;
  }>;
}


export default function MapPage() {
  const [stations, setStations] = useState<Station[]>([]);
  const [filteredStations, setFilteredStations] = useState<Station[]>([]);
  const [selectedStation, setSelectedStation] = useState<Station | null>(null);
  const [selectedStationDetail, setSelectedStationDetail] = useState<StationDetail | null>(null);
  
  // Filters
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedConnector, setSelectedConnector] = useState<string | null>(null);
  const [minPower, setMinPower] = useState<number>(22);
  
  // Loading & error states
  const [loading, setLoading] = useState(false);
  const [detailLoading, setDetailLoading] = useState(false);

  // Fetch stations on load
  useEffect(() => {
    const fetchStations = async () => {
      setLoading(true);
      try {
        const response = await fetch('/api/stations?page=1&pageSize=50');
        if (response.ok) {
          const data = await response.json();
          setStations(data.items || []);
          setFilteredStations(data.items || []);
        }
      } catch (error) {
        console.error('Error fetching stations:', error);
      } finally {
        setLoading(false);
      }
    };
    fetchStations();
  }, []);

  // Fetch station detail when selectedStation changes
  useEffect(() => {
    if (!selectedStation) {
      setSelectedStationDetail(null);
      return;
    }
    const fetchDetail = async () => {
      setDetailLoading(true);
      try {
        const response = await fetch(`/api/stations/${selectedStation.ocmId}`);
        if (response.ok) {
          const data = await response.json();
          setSelectedStationDetail(data);
        } else {
          // Fallback mockup detail using the selected station data
          setSelectedStationDetail({
            id: String(selectedStation.ocmId),
            ocmId: selectedStation.ocmId,
            name: selectedStation.name,
            address: selectedStation.address,
            city: selectedStation.city,
            district: selectedStation.district,
            latitude: selectedStation.latitude,
            longitude: selectedStation.longitude,
            operatorName: selectedStation.operatorName,
            statusText: selectedStation.statusText,
            averageRating: 4.8,
            reviewCount: 12,
            connectors: [
              {
                connectionType: selectedStation.name.includes('DC') ? 'CCS (Type 2)' : 'Type 2',
                powerKW: selectedStation.name.includes('DC') ? 150 : 22,
                currentType: selectedStation.name.includes('DC') ? 'DC' : 'AC',
                quantity: selectedStation.connectorCount,
                status: 'Available'
              }
            ]
          });
        }
      } catch (error) {
        console.error('Error fetching detail, falling back:', error);
      } finally {
        setDetailLoading(false);
      }
    };
    fetchDetail();
  }, [selectedStation]);

  // Apply filters
  useEffect(() => {
    let result = stations;

    // Search query
    if (searchQuery.trim() !== '') {
      const query = searchQuery.toLowerCase();
      result = result.filter(s => 
        s.name.toLowerCase().includes(query) || 
        s.city.toLowerCase().includes(query) ||
        s.district.toLowerCase().includes(query)
      );
    }

    // Connector type
    if (selectedConnector) {
      if (selectedConnector === 'CCS 2') {
        result = result.filter(s => s.name.includes('DC') || s.name.includes('CCS'));
      } else if (selectedConnector === 'Type 2') {
        result = result.filter(s => !s.name.includes('DC'));
      }
    }

    setFilteredStations(result);
  }, [searchQuery, selectedConnector, minPower, stations]);

  // Map Default Position (Turkey Center or first station)
  const defaultPosition: [number, number] = selectedStation 
    ? [selectedStation.latitude, selectedStation.longitude]
    : [39.9334, 32.8597]; // Ankara Coordinates

  return (
    <main className="relative h-full w-full overflow-hidden flex flex-row flex-grow">
        
        {/* Interactive Map (Leaflet) */}
        <div className="absolute inset-0 z-0 h-full w-full">
          <MapContainer 
            center={defaultPosition} 
            zoom={selectedStation ? 12 : 6} 
            zoomControl={false}
            className="w-full h-full"
          >
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            {filteredStations.map((station) => (
              <Marker 
                key={station.ocmId} 
                position={[station.latitude, station.longitude]}
                icon={station.name.includes('DC') ? greenIcon : defaultIcon}
                eventHandlers={{
                  click: () => {
                    setSelectedStation(station);
                  },
                }}
              >
                <Popup>
                  <div className="font-sans">
                    <h4 className="font-bold text-on-surface text-label-md">{station.name}</h4>
                    <p className="text-[11px] text-on-surface-variant mt-1">{station.address}</p>
                    <p className="text-[11px] font-semibold text-secondary mt-1">Status: {station.statusText}</p>
                  </div>
                </Popup>
              </Marker>
            ))}
            {selectedStation && (
              <ChangeMapView center={[selectedStation.latitude, selectedStation.longitude]} />
            )}
          </MapContainer>
        </div>

        {/* Desktop Sidebar Overlay */}
        <aside className="hidden md:flex absolute left-8 top-24 bottom-12 w-96 flex-col z-[500]">
          <div className="glass-panel rounded-2xl shadow-xl flex flex-col h-full overflow-hidden">
            {/* Search & Filters */}
            <div className="p-6 space-y-6 bg-surface/40 border-b border-outline-variant/20">
              <div className="relative">
                <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant">search</span>
                <input 
                  className="w-full pl-10 pr-4 py-3 bg-surface border border-outline-variant/50 rounded-xl focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all text-body-md outline-none" 
                  placeholder="Search charging stations..." 
                  type="text"
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                />
              </div>

              <div className="space-y-3">
                <p className="font-bold text-[11px] text-on-surface-variant uppercase tracking-wider">Connector Types</p>
                <div className="flex flex-wrap gap-2">
                  <button 
                    onClick={() => setSelectedConnector(selectedConnector === 'CCS 2' ? null : 'CCS 2')}
                    className={`px-4 py-2 rounded-lg text-label-md font-medium transition-colors ${
                      selectedConnector === 'CCS 2' 
                        ? 'bg-primary text-white' 
                        : 'bg-surface-variant/50 text-on-surface-variant hover:bg-surface-variant'
                    }`}
                  >
                    CCS 2 (DC)
                  </button>
                  <button 
                    onClick={() => setSelectedConnector(selectedConnector === 'Type 2' ? null : 'Type 2')}
                    className={`px-4 py-2 rounded-lg text-label-md font-medium transition-colors ${
                      selectedConnector === 'Type 2' 
                        ? 'bg-primary text-white' 
                        : 'bg-surface-variant/50 text-on-surface-variant hover:bg-surface-variant'
                    }`}
                  >
                    Type 2 (AC)
                  </button>
                </div>
              </div>

              <div className="space-y-3">
                <div className="flex justify-between items-center">
                  <p className="font-bold text-[11px] text-on-surface-variant uppercase tracking-wider">Min Power (kW)</p>
                  <span className="text-primary font-bold text-label-md">{minPower}kW+</span>
                </div>
                <input 
                  className="w-full h-2 bg-surface-variant rounded-lg appearance-none cursor-pointer accent-primary" 
                  type="range"
                  min="22"
                  max="350"
                  step="22"
                  value={minPower}
                  onChange={(e) => setMinPower(Number(e.target.value))}
                />
              </div>
            </div>

            {/* Station Results List */}
            <div className="flex-1 overflow-y-auto custom-scrollbar p-4 space-y-4">
              {loading ? (
                <div className="text-center py-8 text-on-surface-variant">Stations loading...</div>
              ) : filteredStations.length === 0 ? (
                <div className="text-center py-8 text-on-surface-variant">No stations found matching filters.</div>
              ) : (
                filteredStations.map((station) => (
                  <div 
                    key={station.ocmId}
                    onClick={() => setSelectedStation(station)}
                    className={`p-4 rounded-xl border cursor-pointer transition-colors ${
                      selectedStation?.ocmId === station.ocmId
                        ? 'border-primary/40 bg-primary/10'
                        : 'border-outline-variant/30 bg-surface/50 hover:bg-surface-variant/30'
                    }`}
                  >
                    <div className="flex justify-between items-start mb-2">
                      <h3 className="font-headline-md text-headline-md font-bold text-on-surface line-clamp-1">{station.name}</h3>
                      <span className={`px-2 py-0.5 rounded text-[9px] font-bold uppercase ${
                        station.name.includes('DC') 
                          ? 'bg-secondary-container text-on-secondary-container'
                          : 'bg-surface-variant text-on-surface-variant'
                      }`}>
                        {station.name.includes('DC') ? 'DC Fast' : 'AC Std'}
                      </span>
                    </div>
                    <p className="text-on-surface-variant text-label-sm mb-3 flex items-center gap-1">
                      <span className="material-symbols-outlined text-[16px]">location_on</span>
                      <span className="line-clamp-1">{station.address}</span>
                    </p>
                    <div className="flex gap-4">
                      <div className="text-center bg-surface p-2 rounded-lg flex-1 border border-outline-variant/20">
                        <p className="text-[9px] text-on-surface-variant uppercase font-medium">Connectors</p>
                        <p className="font-bold text-secondary text-label-md">{station.connectorCount}</p>
                      </div>
                      <div className="text-center bg-surface p-2 rounded-lg flex-1 border border-outline-variant/20">
                        <p className="text-[9px] text-on-surface-variant uppercase font-medium">Status</p>
                        <p className={`font-bold text-label-md ${
                          station.statusText === 'Active' ? 'text-secondary' : 'text-error'
                        }`}>{station.statusText}</p>
                      </div>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        </aside>

        {/* Floating Selected Station Card */}
        {selectedStation && selectedStationDetail && (
          <div className="absolute bottom-32 md:bottom-12 right-4 left-4 md:left-auto md:right-12 z-[1000] max-w-md w-full">
            <div className="glass-panel p-6 rounded-2xl shadow-2xl animate-in slide-in-from-bottom-4 duration-500">
              <div className="flex justify-between items-start mb-4">
                <div>
                  <h2 className="font-headline-md text-headline-md text-on-background mb-1">{selectedStationDetail.name}</h2>
                  <p className="text-on-surface-variant text-body-md line-clamp-1">{selectedStationDetail.address}, {selectedStationDetail.city}</p>
                </div>
                <button 
                  onClick={() => setSelectedStation(null)}
                  className="p-1 bg-surface-variant/40 rounded-full hover:bg-surface-variant transition-colors"
                >
                  <span className="material-symbols-outlined text-[20px]">close</span>
                </button>
              </div>

              {detailLoading ? (
                <div className="py-4 text-center text-on-surface-variant">Loading details...</div>
              ) : (
                <>
                  <div className="grid grid-cols-2 gap-4 mb-6">
                    <div className="p-4 rounded-xl bg-surface-container border border-outline-variant/30">
                      <div className="flex items-center gap-2 mb-1">
                        <span className="material-symbols-outlined text-secondary" style={{ fontVariationSettings: "'FILL' 1" }}>bolt</span>
                        <span className="font-bold text-label-sm text-on-surface-variant">Connector Type</span>
                      </div>
                      <p className="text-body-lg font-bold text-on-surface">
                        {selectedStationDetail.connectors[0]?.connectionType || 'CCS 2'}
                      </p>
                    </div>
                    <div className="p-4 rounded-xl bg-surface-container border border-outline-variant/30">
                      <div className="flex items-center gap-2 mb-1">
                        <span className="material-symbols-outlined text-primary">speed</span>
                        <span className="font-bold text-label-sm text-on-surface-variant">Max Power</span>
                      </div>
                      <p className="text-body-lg font-bold text-on-surface">
                        {selectedStationDetail.connectors[0]?.powerKW || 22} kW
                      </p>
                    </div>
                  </div>

                  <div className="flex gap-3">
                    <button className="flex-1 bg-primary hover:bg-surface-tint text-white py-4 rounded-xl font-bold text-body-lg shadow-lg shadow-primary/30 active:scale-95 transition-transform flex items-center justify-center gap-2">
                      <span className="material-symbols-outlined">flash_on</span>
                      <span>Start Charging</span>
                    </button>
                    <button className="w-14 h-14 border-2 border-outline-variant flex items-center justify-center rounded-xl hover:bg-surface-variant/20 transition-colors">
                      <span className="material-symbols-outlined text-outline">favorite</span>
                    </button>
                  </div>
                </>
              )}
            </div>
          </div>
        )}
      </main>
  );
}
