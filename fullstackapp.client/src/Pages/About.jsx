
import { useState, useEffect, useRef } from 'react';
import L from 'leaflet';
import "leaflet/dist/leaflet.css";

import 'leaflet-routing-machine/dist/leaflet-routing-machine.css';
import 'leaflet-routing-machine';
function About() {

    const mapContainerRef = useRef(null);
    const map = useRef(null);
    const [lat1] = useState(-37.8813);
    const [lng] = useState(145.1652);
    const [zoom] = useState(12);
    const marker = useRef(null);
    const marker2 = useRef(null);

    const [address, setAddress] = useState('');

    useEffect(() => {

        map.current = L.map(mapContainerRef.current, {
            center: [lat1, lng],
            zoom: 14
        })

        // add marker
        marker.current = L.marker([-37.8813, 145.1652]).addTo(map.current);
        marker.current.bindPopup("<b>Village Cinemas</b><br>", {
            autoClose: false,
            closeOnClick: false
        }).openPopup();

        // add layer
        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
            attribution: "&copy; <a href='https://www.openstreetmap.org/copyright'>OpenStreetMap</a> contributors",
        }).addTo(map.current);

        //set view
        //map.current.setView([lat, lng], zoom);

        //map.current.invalidateSize();

        return () => map.current.remove();

    }, [lat1, lng, zoom]);

    // search routes

    const handleSearch = async (e) => {
        e.preventDefault();
        if (!address) return;

        try {
            const response = await fetch(
                `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(address)}`
            );
            const data = await response.json();

            if (data.length > 0) {
                // lat and lon returned from data for geocoding address
                const { lat, lon } = data[0];
                
                map.current.setView([lat, lon], 14);

                // Remove previous marker
                if (marker2.current) {
                    map.current.removeLayer(marker2.current);
                }

                marker2.current = L.marker([lat, lon]).addTo(map.current)
                    .bindPopup('Search result').openPopup();

                // Add routing feature
                L.Routing.control({
                    waypoints: [
                        L.latLng(lat1, lng),
                        L.latLng(lat, lon)
                    ],
                    routeWhileDragging: true
                }).addTo(map.current);

                console.log("lat", lat)

                
            } else {
                alert('Address not found');
            }
        } catch (err) {
            console.error('Geocode error:', err);
        }
    }

    return (
        <div>
            <h2>About</h2>
            <p> The location of a cinema to watch the movies. For website project demonstration purposes only.</p>
            <div ref={mapContainerRef} style={{ height: '400px', width: '400px', position: 'relative', margin: '0 auto' }} />

            <form onSubmit={handleSearch} style={{ padding: '10px', backgroundColor: '#fff' }}>
            <input
                type="text"
                value={address}
                placeholder="Enter address..."
                onChange={(e) => setAddress(e.target.value)}
                style={{ width: '70%', padding: '8px' }}
            />
            <button type="submit" style={{ padding: '8px 12px' }}>Search address to get route</button>
        </form>
        </div>
    );
};

export { About }