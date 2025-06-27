
import { useAuth } from '../AuthContext';
import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from "react-router"

// calendar
import { Calendar } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import listPlugin from '@fullcalendar/list';

// material ui for dialog box
import Dialog from '@mui/material/Dialog';
import DialogContentText from "@mui/material/DialogContentText";
import DialogTitle from '@mui/material/DialogTitle';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';

import { SeatBooking } from '../Component/SeatBooking.jsx';

// material ui for success popup
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';

const Alert = React.forwardRef(function Alert(props, ref) {
    return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

function Bookings() {

    // user details and movie chosen from other pages
    const { movie, user } = useAuth();


    // session data for movie
    const [sessions, setSessions] = useState();
    const [sessions2, setSessions2] = useState();


    const calendarRef = useRef(null);
    const sessionIdSelect = useRef()


    // user current bookings
    const [userBookings, setUserBookings] = useState();

    let navigate = useNavigate();

    // Dialog popup box
    const [open, setOpen] = useState(false);
    const [msg,setMsg] = useState("")

    const [moviechoice, setMoviechoice] = useState("Movie selected")
    const [sessionchoice, setSessionchoice] = useState("2025-03-21T14:00:00");


    const [selectedSeats, setSelectedSeats] = useState([]);

    const handleSeatChange = (updatedSelection) => {
        setSelectedSeats(updatedSelection);
    };

    // Show booking Popup when user clicks session on calendar
    function handleClickToOpenPopup(info) {
        setOpen(true);

        setMsg("");

        if (user == undefined) {
            setMsg("User not signed in. Please sign in to book seats.")
        }
        // when user clicks event, detail saved
        setMoviechoice(info.event.title);

        sessionIdSelect.current = info.event.id;

        // show a format local date string to user
        const viewStartDate = new Date(info.event.start)
        const viewEndDate = new Date(info.event.end)
        
        setSessionchoice(viewStartDate.toLocaleString() + " to " + viewEndDate.toLocaleString() )
        
    };

    const handleToClosePopup = () => {
        setOpen(false);
    };


    async function handleTicket() {
        const result = await sendBooking();
        if (result == "ok") {
            
            setOpen(false);
        }
        else {
            // pass
        }
    };


    async function sendBooking() {

        // user redirected from booking if they have not logged in
        if (user == undefined) {
            navigate('/login');
        }

        console.log("user obj:",user)
        try {

            const ticketBody = JSON.stringify(
                {
                    seatNumber: selectedSeats.map(i => i + 1).join(','),
                    sessionId: sessionIdSelect.current,
                    userId: user.userId
                })
            console.log("ticket body:", ticketBody)

            const response = await fetch('api/Bookings', {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },

                // Send the booking request
                /* body
                {
                seatNumber:
                sessionId:
                userId:
                }
                */
                body: ticketBody
            });

            if (response.status === 201) {

                console.log("Resource created successfully.");
                console.log("debug:", response)

                handleSubmit()

                return "ok"
           
            }
            else {
                
                console.error("Failed to create resource. Status:", response.status);

                setMsg("Error in booking")

                const data = await response.json();  
                console.log("data:", data)

                if (data.message) {
                    setMsg(data.message)
                }

                return "fail"
            }
        }
        catch (e) {
            console.log("Error sending request:",e)
        }
    }

    // Populate Session data
    useEffect(() => {
        populateSessionData();
    }, [movie]);

    useEffect(() => {

        populateSessionData2();
        console.log("Sessions2", sessions2);
    }, [sessions]);

    // Create calendar

    useEffect(() => {
        // Ensure the calendar is only initialized after the component mounts
        const calendar = new Calendar(calendarRef.current, {
            plugins: [dayGridPlugin, timeGridPlugin, listPlugin],
            initialView: 'dayGridMonth',
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,listWeek'
            },
            events: sessions2,
            eventClick: handleClickToOpenPopup
        });

        calendar.render(); // Render the calendar

        // Cleanup function to destroy the calendar on component unmount
        return () => {
            calendar.destroy();
        };
    }, [sessions2]);


    // Function to fetch data
    const fetchUserBookings = async () => {
        if (user != null) {
            const response = await fetch(`api/Bookings/GetBookingsForUser/${user.userId}`);
            if (response.ok) {
                console.log("debug user bookings:", response)
                const data = await response.json();
                console.log("data user bookings:", data)

                // change date format
                const data2 = data.map((item) => ({
                    ...item,
                    startDatetime: new Date(item.startDatetime).toLocaleString(),
                    endDatetime: new Date(item.endDatetime).toLocaleString()
                }));

                setUserBookings(data2);
            }
        }
    };

    // Fetch the user bookings data
    useEffect(() => {

        const load = async () => {
            await fetchUserBookings(); // Immediate fetch
        };

        load(); // initial load

        const intervalId = setInterval(() => {
            fetchUserBookings(); // Fetch data every 30 seconds
        }, 30000); // 30,000 ms = 30 seconds

        // Clean up the interval when the component unmounts
        return () => clearInterval(intervalId);
    }, []);

    const userBookingsTable = userBookings == undefined
        ? <p></p>
        : <table style={{ marginLeft: 'auto', marginRight: 'auto' }} className="table table-striped" aria-labelledby="tableLabel">
            <thead>
                <tr>
                    <th>Title</th>
                    <th>Seat Number</th>
                    <th>StartDatetime</th>
                    <th>EndDatetime</th>
                </tr>
            </thead>
            <tbody>
                {userBookings.map((item, index) =>
                    <tr key={index}>
                        <td>{item.title}</td>
                        <td>{item.seatNumber}</td>
                        <td>{item.startDatetime}</td>
                        <td>{item.endDatetime}</td>
                    </tr>
                )}
            </tbody>
        </table>

    const bookingHeading = user == undefined ? <p></p>
        : <p style={{color: 'red'}}><b>Current bookings by {user.username?.trim()} </b></p>


    async function populateSessionData() {
        const response = await fetch(`api/GetSessionTimes/ByMovie/${movie.id}`);
        if (response.ok) {
            console.log("debug:", response)
            const data = await response.json();
            console.log("data:", data)
            setSessions(data);
        }
    }

    // edit the fields in the session list for displaying in calendar
    function populateSessionData2() {
        if (sessions) {
            const result = sessions.map((item) => {
                delete item.movieId;
                delete item.movies;
                item.title = movie.title;
                return item;
            })
            console.log("result",result)
            setSessions2(result)

        }
    }

    const seatDropdownApi = `api/Bookings/GetSeatsForSession/${sessionIdSelect.current}`

    // Popup for success booking

    const [openSnackbar, setOpenSnackbar] = useState(false);
    const [snackbarMessage, setSnackbarMessage] = useState('');
    const [snackbarSeverity, setSnackbarSeverity] = useState('success'); // 'success', 'error', 'warning', 'info'

    const handleSubmit = () => {
        
        // Simulate an API call or some successful operation
        setTimeout(() => {
            setSnackbarMessage('Your form was submitted successfully!');
            setSnackbarSeverity('success');
            setOpenSnackbar(true);
        }, 1000);
    };

    const handleCloseSnackbar = (event, reason) => {
        if (reason === 'clickaway') {
            return; // Prevent closing when clicking outside the Snackbar
        }
        setOpenSnackbar(false);
    };


    return (<div  >
        <h2>Bookings</h2>

        <p>Title: {movie.title}</p>
        <p style={{ width: '50%', marginLeft: 'auto',  
            marginRight: 'auto', 
            textAlign: 'center'
        }}>
            Description: {movie.description}</p>
        <p>Genre: {movie.genres}</p>
        <p>Esrb rating:{movie.esrbRating}</p>
        <p>Runtime: {movie.runtime}</p>


        <br></br>

        {bookingHeading}

        {userBookingsTable}

        <div ref={calendarRef} />

        {/* Dialog box for booking session*/ }
        <div style={{}}>

            <Dialog open={open} onClose={handleToClosePopup}>
                <DialogTitle>{"Book Ticket in Session"}</DialogTitle>
                <DialogContent>
                    <DialogContentText style={{ color: 'red', fontWeight: 'bold' }}>
                        {msg}
                    </DialogContentText>
                    <SeatBooking endpoint={seatDropdownApi} value={selectedSeats} onChange={handleSeatChange}> </SeatBooking>

                    <DialogContentText>
                        {moviechoice}
                    </DialogContentText>
                    <DialogContentText>
                        {sessionchoice}
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <button onClick={handleTicket}>
                        Buy Ticket
                    </button>
                    <button onClick={handleToClosePopup}>
                        Cancel
                    </button>
                </DialogActions>
            </Dialog>

            <Snackbar
                open={openSnackbar}
                autoHideDuration={6000} // How long it stays open in ms
                onClose={handleCloseSnackbar}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }}
            >
                <Alert
                    onClose={handleCloseSnackbar}
                    severity={snackbarSeverity}
                    sx={{ width: '100%' }} // Full width of Snackbar
                >
                    {snackbarMessage}
                </Alert>
            </Snackbar>

        </div>



    </div>)


}

export { Bookings }


