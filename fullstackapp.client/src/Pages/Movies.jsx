import '../Image.css';
import '../Movies.css';

import { useAuth } from '../AuthContext';
import { useNavigate } from "react-router";
import { useState, useEffect } from 'react';

function Movies() {
    const [movielist, setMovielist] = useState();
    const [movielist2, setMovielist2] = useState();
    let navigate = useNavigate();
    const { setMovie } = useAuth();


    useEffect(() => {
        populateMovielist();
    }, []);


    useEffect(() => {
        updateImage();
    }, [movielist]);

    // Edit the filename prop to later load each image from assets folder


    
    var movieGrid = <p>Loading</p>

    if (movielist2 != undefined) {
        movieGrid = movielist2.map(
            (item) =>
            (
                <div key={item.id} style={{
                    width: '270px',
                }}>

                    <img src={item.movieImageFilename}
                        className='picture-resize'
                        onClick={() => {
                            setMovie(item); // set movie chosen
                            navigate('/bookings');
                        }} />
                    <p style={{
                        width: '270px',

                    }} >{item.title}</p>
                </div>
            )
        )
    }
    
    return (<div style={{ flexGrow: '1' }}>
        <h2>Movies</h2>
        
        <div className='grid-container'>
            {movieGrid}
        </div>

    </div>)


     async function populateMovielist() {
        const response = await fetch('api/Movies');
         if (response.ok) {
            
             const data = await response.json();
             
             setMovielist(data);
             
             }
    }

    function updateImage() {
        if (movielist) {
            const result = movielist.map((item) => {

        
                item['movieImageFilename'] = new URL(`../assets/${item.movieImageFilename}`, import.meta.url).href;
                
                return item;
            })
            setMovielist2(result);
        }
    }
}




export { Movies }