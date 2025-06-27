import { useEffect, useRef, useState } from 'react';
import Chart from 'chart.js/auto'
import autocolors from 'chartjs-plugin-autocolors'; // npm install chartjs-plugin-autocolors
import { Colors } from 'chart.js';
function ChartStat() {

    const chartRef = useRef(null);
    const chartRefPie = useRef(null);

    const [movieTitleList, setMovieTitleList] = useState(["",""])
    const [ticketCountList, setTicketCountList] = useState(["100","200"])

    // Create a line chart

    async function fetchTicketData()
    {
        
        const response = await fetch('api/Bookings/GetBookingsForMovies');
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        var ticketData = await response.json();

        var tempMovie = []
        var tempTicket = []

        for (const item of ticketData) {
            tempMovie.push(item.movieTitle);
            tempTicket.push(item.ticketCount.toString());
        }

        setMovieTitleList(tempMovie);
        setTicketCountList(tempTicket);
        
    }

    useEffect(() => {
        // populate data

        const load = async () => {
            await fetchTicketData(); // Immediate fetch
        };

        load(); // initial load})
    },[])


    useEffect(() => {

        const ctx = chartRef.current.getContext('2d');

        //Between different datasets
        //Chart.register(Colors);

        // generate auto colors per column
        Chart.register(autocolors);

        console.log("movie title list", movieTitleList)

        const newChart = new Chart(ctx, {
            type: 'bar',
            data: {
                
                labels: movieTitleList,
                datasets: [{
                    label: '# of Tickets',
                    data: ticketCountList,
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                },
                plugins: {
                    
                    autocolors: {
                        mode: 'data'
                    },

                    title:
                    {
                        display: true,
                        text: 'Statistics of movies chosen by users',
                        padding: {
                            top: 20,
                            bottom: 20
                        }
                    }
         
                }
            }
        });

        return () => {
            newChart.destroy();
        };


    }, [movieTitleList,ticketCountList]);

    // Pie chart

    useEffect(() => {

        const ctx = chartRefPie.current.getContext('2d');

        //Between different datasets
        Chart.register(Colors);

        const newChartPie = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: movieTitleList,
                datasets: [{
                    label: 'Tickets',
                    data: ticketCountList,
                    borderWidth: 1
                }]
            },
            options: {
               
                plugins: {
              
                    title:
                    {
                        display: true,
                        text: 'No# tickets bought for each movie',
                        padding: {
                            top: 20,
                            bottom: 10
                        }
                    },

                    colors:
                   {
                        enabled: true,
                        forceOverride: true
                   }
                   
                }
            }
        });

        return () => {
            newChartPie.destroy();
        };


    }, [movieTitleList, ticketCountList]);

    return (<div>
        <h2>Chart statistics</h2>
        <canvas ref={chartRef} width="400" height="400" style={{marginBottom: '20px'}}></canvas>

        <canvas ref={chartRefPie} width="400" height="400" style={{ marginBottom: '20px' }}></canvas>
    </div>)

}

export { ChartStat }