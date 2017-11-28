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
import styles from './scss/index';
import {headerReducer} from './Components/Header/index';
import { StatisticsPage , statisticsReducer} from './Scenes/Statistics/index';
import createBrowserHistory from 'history/createBrowserHistory'

let initialState = {
    loginPage: {
    },
    header : {},
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
    gameRoom: gameRoomReducer,
    header : headerReducer,
    statistics: statisticsReducer
}), initialState, applyMiddleware(apiMiddleware, logger, websocket));

let history = createBrowserHistory();

render(
    <Provider store={store}>
        <BrowserRouter>
            <Switch>
                <Route exact path="/" component={Home} />
                <Route path="/login" component={LoginPage} />
                <Route path="/register" component={RegisterPage} />
                <Route path="/gameroom" component={AppGameRoom} />
                <Route path="/statistics" component={StatisticsPage}/>
            </Switch>
        </BrowserRouter>
    </Provider>,
    document.getElementById('root')
);