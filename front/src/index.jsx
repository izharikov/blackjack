import React from 'react'
import { render } from 'react-dom'
import { Provider } from 'react-redux'
import { createStore, applyMiddleware, combineReducers } from 'redux'
import helloReducer from './reducers'
import { BrowserRouter, Route, Router, Switch } from "react-router-dom";
import { loginReducer } from './Components/LoginForm/login';
import { apiMiddleware } from 'redux-api-middleware';
import logger from './middleware/logger';
import Home from './Scenes/Home/index';
import websocket from './middleware/websocket';
import AppGameRoom, { gameRoomReducer } from './Scenes/GameRoom/index';
import { RegisterPage, LoginPage} from './Scenes/Authentification/index';
import { registerReducer } from './Components/RegisterForm/index';

let initialState = {
    loginPage: {
    },
    registerPage : {},
    gameRoom: {
        gameState:{},
        currentUser: {
            place: -1
        }
    }
}

let store = createStore(combineReducers({
    loginPage: loginReducer,
    registerPage : registerReducer,
    gameRoom: gameRoomReducer
}), initialState, applyMiddleware(apiMiddleware, logger, websocket));

render(
    <Provider store={store}>
        <BrowserRouter>
            <Switch>
                <Route exact path="/" component={Home} />
                <Route path="/login" component={LoginPage} />
                <Route path="/register" component={RegisterPage} />
                <Route path="/gameroom" component={AppGameRoom} />
            </Switch>
        </BrowserRouter>
    </Provider>,
    document.getElementById('root')
);