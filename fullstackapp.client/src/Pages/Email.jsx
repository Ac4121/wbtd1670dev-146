import React, { useEffect, useState, useRef } from 'react';
import { gapi } from 'gapi-script';

const DISCOVERY_DOC = 'https://www.googleapis.com/discovery/v1/apis/gmail/v1/rest';
const SCOPES = 'https://www.googleapis.com/auth/gmail.readonly https://www.googleapis.com/auth/gmail.send';

function Gmail() {
    const [gapiLoaded, setGapiLoaded] = useState(false);
    const [gisLoaded, setGisLoaded] = useState(false);
    const [isAuthorized, setIsAuthorized] = useState(false);
    const [labelsOutput, setLabelsOutput] = useState('');
    const tokenClientRef = useRef(null);

    const toRef = useRef();
    const subjectRef = useRef();
    const messageRef = useRef();

    
    // Load Google scripts
    useEffect(() => {
        // get api key
        var CLIENT_ID = "";
        var API_KEY = "";

        const fetchClientId = async () => {

            const api_response = await fetch(`api/Keys/GetGmailKeys`);
            if (api_response.ok) {
                console.log("debug gmail keys:", api_response)
                const data = await api_response.json();
                console.log("data gmail keys:", data.response)

                CLIENT_ID = data.response.GOOGLE_CLIENT_ID;
                API_KEY = data.response.GMAIL_API_KEY;
                console.log("client",CLIENT_ID);
                console.log("api_key",API_KEY)
            }

        };
        fetchClientId();

        const initializeGapiClient = async () => {
            console.log("api key", API_KEY)

            await window.gapi.client.init({
                apiKey: API_KEY,
                discoveryDocs: [DISCOVERY_DOC],
            });
            setGapiLoaded(true);
        };


        const loadScript = (src, onLoad) => {
            const script = document.createElement('script');
            script.src = src;
            script.async = true;
            script.defer = true;
            script.onload = onLoad;
            document.body.appendChild(script);
        };


        
        loadScript('https://apis.google.com/js/api.js', () => {
            window.gapi.load('client', initializeGapiClient);
        });

        loadScript('https://accounts.google.com/gsi/client', () => {
            console.log("client id",CLIENT_ID)
            tokenClientRef.current = window.google.accounts.oauth2.initTokenClient({
                client_id: CLIENT_ID,
                scope: SCOPES,
                callback: '', // defined in handleAuthClick
            });
            setGisLoaded(true);
        });
    }, []);



    useEffect(() => {
        if (gapiLoaded && gisLoaded) {
            // Both libraries loaded
        }
    }, [gapiLoaded, gisLoaded]);

    const handleAuthClick = () => {
        tokenClientRef.current.callback = async (resp) => {
            if (resp.error !== undefined) {
                console.error(resp);
                throw resp;
            }
            setIsAuthorized(true);
            await listLabels();
        };

        const token = window.gapi.client.getToken();
        if (!token) {
            tokenClientRef.current.requestAccessToken({ prompt: 'consent' });
        } else {
            tokenClientRef.current.requestAccessToken({ prompt: '' });
        }
    };

    const handleSignoutClick = () => {
        const token = window.gapi.client.getToken();
        if (token) {
            window.google.accounts.oauth2.revoke(token.access_token);
            window.gapi.client.setToken('');
            setLabelsOutput('');
            setIsAuthorized(false);
        }
    };

    const createEmail = (to, subject, message) => {
        return [
            'Content-Type: text/plain; charset="UTF-8"\n',
            'MIME-Version: 1.0\n',
            `To: ${to}\n`,
            `Subject: ${subject}\n\n`,
            message,
        ].join('');
    };

    const sendEmail = (e) => {
        e.preventDefault();

        const to = toRef.current.value;
        const subject = subjectRef.current.value;
        const message = messageRef.current.value;

        const rawEmail = createEmail(to, subject, message);
        const base64Email = window.btoa(rawEmail).replace(/\+/g, '-').replace(/\//g, '_');

        window.gapi.client.gmail.users.messages.send({
            userId: 'me',
            resource: { raw: base64Email },
        }).then((response) => {
            alert('Email sent!');
            console.log('Email response:', response);
        });
    };

    const listLabels = async () => {
        try {
            const response = await window.gapi.client.gmail.users.labels.list({ userId: 'me' });
            const labels = response.result.labels;
            if (!labels || labels.length === 0) {
                setLabelsOutput('No labels found.');
            } else {
                setLabelsOutput('Labels:\n' + labels.map(label => label.name).join('\n'));
            }
        } catch (err) {
            setLabelsOutput(err.message);
        }
    };

    return (
        <div>
            <h2>Send email using gmail API</h2>

            <button onClick={handleAuthClick}>Authorize</button>
            <button onClick={handleSignoutClick} style={{ visibility: isAuthorized ? 'visible' : 'hidden' }}>
                Sign Out
            </button>

            {isAuthorized && (
                <form onSubmit={sendEmail} style={{ marginTop: '1em' }}>
                    <label>
                        Recipient:<br />
                        <input type="email" ref={toRef} required />
                    </label><br /><br />

                    <label>
                        Subject:<br />
                        <input type="text" ref={subjectRef} required />
                    </label><br /><br />

                    <label>
                        Message:<br />
                        <textarea rows="4" cols="50" ref={messageRef} required></textarea>
                    </label><br /><br />

                    <button type="submit">Send Email</button>
                </form>
            )}

            <pre style={{ whiteSpace: 'pre-wrap', marginTop: '2em' }}>{labelsOutput}</pre>
        </div>
    );
};

export { Gmail };