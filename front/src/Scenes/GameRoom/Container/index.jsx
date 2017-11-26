import { WEBSOCKET_CONNECTION_REQUEST , WEBSOCKET_MESSAGE_SEND} from '../../../middleware/websocket';
import connect from "react-redux/es/connect/connect";
import {ONMESSAGE_ACTION_TYPE} from '../index';

const mapStateToProps = (state, ownProps) => {
    return state.gameRoom;
};

const blackjackSocketUrl = `ws://${location.host}/ws/blackjack`;

const mapDispatchToProps = (dispatch, ownProps) => {
    return {
        connectToWebsocket: () => {
            dispatch({
                type: WEBSOCKET_CONNECTION_REQUEST,
                url: blackjackSocketUrl,
                onmessage: ONMESSAGE_ACTION_TYPE
            });
        },
        sendMessage: (message) => {
            dispatch({
                type: WEBSOCKET_MESSAGE_SEND,
                url: blackjackSocketUrl,
                payload: {
                    text: message
                }
            })
        }
    };
};

export const createConnectedComponent = (_gameroom) => {
    const AppGameRoom = connect(mapStateToProps, mapDispatchToProps)(_gameroom);
    return AppGameRoom;
}