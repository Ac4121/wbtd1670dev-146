import React, { useState, useEffect } from 'react';
import '../Booking.css';

function SeatBooking({ endpoint , value = [], onChange }) {
    const rows = 10;
    const cols = 10;
    const totalSeats = rows * cols;

    const [availableSeats, setAvailableSeats] = useState([]);
    const [seats, setSeats] = useState([]);


    // Function to fetch data
    const fetchOptions = async () => {
        try { 
            const response = await fetch(endpoint);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const data = await response.json(); // Assuming the response is an array of numbers

            setAvailableSeats(data);

            }
            catch(error) {
                console.error('Error fetching seats:', error);
            };
        }


    // Fetch the data when the component mounts
    useEffect(() => {

        const load = async () => {
            await fetchOptions(); // Immediate fetch
        };

        load(); // initial load

        const intervalId = setInterval(() => {
            fetchOptions(); // Fetch data every 30 seconds
        }, 30000); // 30,000 ms = 30 seconds

        // Clean up the interval when the component unmounts
        return () => clearInterval(intervalId);
    }, []);


    useEffect(() => {
        const initialSeats = Array.from({ length: totalSeats }, (_, index) =>
            availableSeats.includes(index) ? 'available' : 'booked'
        );
        setSeats(initialSeats);

    }, [availableSeats]);

    // Toggle booking state
    const toggleSeat = (index) => {
        if (!availableSeats.includes(index)) return; // ignore booked

        const isSelected = value.includes(index);

        const updatedSelection = isSelected
            ? value.filter((i) => i !== index)
            : [...value, index];

        onChange?.(updatedSelection); // only call if `onChange` is provided
        // Send seats selected in format "1,2,3"
    };

    return (
        <div>
        <div className="seat-container">
                {seats.map((status, index) => {

                {/* Check if seat is selected from 'value' */ }
                const isSelected = value.includes(index);

                const seatClass = [
                    'seat',
                    status,
                    isSelected ? 'selected' : ''
                ].join(' ');

                return (
                    <div
                        key={index}
                        className={seatClass}
                        onClick={() => toggleSeat(index)}
                    >
                        {index + 1}
                    </div>
                );
            })}
        </div>
            <p>Selected seats: {value.map(i => i + 1).join(',')}</p>
        </div>
    );
};

export { SeatBooking };