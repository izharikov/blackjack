export const WEBSOCKET_CONNECTION_REQUEST = "WEBSOCKET_CONNECTION_REQUEST ";
export const WEBSOCKET_MESSAGE_SEND = "WEBSOCKET_MESSAGE_SEND";

const _websockets = {};

const websocket = store => next => action => {
    if (action.type == WEBSOCKET_CONNECTION_REQUEST) {
        console.log('Websocket request');
        var websocket = new WebSocket(action.url);
        _websockets[action.url] = websocket;
        websocket.onmessage = function (event) {
            return store.dispatch({
                type: action.onmessage,
                payload: event.data
            })
        }
    } else if (action.type == WEBSOCKET_MESSAGE_SEND) {
        let websocket = _websockets[action.url];
        websocket.send(JSON.stringify(action.payload));
    } else {
        return next(action);
    }
}

export default websocket;