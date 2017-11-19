import React, { Component } from 'react'
import { Link } from 'react-router-dom';
import styles from './scss/index';
import connect from "react-redux/es/connect/connect";
import { CALL_API } from "redux-api-middleware";
import rest from '../../common/rest';

class HeaderComponent extends Component {
    constructor(props) {
        super(props);
        let {callUserInfo} = props;
        this.callUserInfo = callUserInfo.bind(this);
    }

    componentWillMount() {
        console.log('it\'s time to call user info')
        this.callUserInfo();
    }

    render() {
        let { loggedIn = false, name } = this.props;
        return <div className={styles.header}>
            {loggedIn && <div>{name}</div>}
            {!loggedIn && <div>
                <Link to="/login">Login</Link>
                <Link to="/register">Register</Link>
            </div>}
        </div>
    }
}

const USER_INFO_REQUEST = "USER_INFO_REQUEST";
const USER_INFO_SUCCESS = "USER_INFO_REQUEST";
const USER_INFO_FAILURE = "USER_INFO_REQUEST";

export const headerReducer = (state, action) => {
    switch (action.type) {
        case USER_INFO_SUCCESS:
            return {
                ...action.payload,
                loggedIn: true
            }
        case USER_INFO_FAILURE:
        default:
            return {
                ...state
            }
    }
}

const userInfoAction = () => {
    return {
        [CALL_API]: {
            endpoint: rest.userInfo,
            method: 'GET',
            headers: { 'Content-Type': 'application/json' },
            types: [USER_INFO_REQUEST, USER_INFO_SUCCESS, USER_INFO_FAILURE],
            credentials: 'same-origin'
        }
    }
}

const mapStateToProp = (state) => {
    console.log('mapStateToProp', state)
    return state.header;
}

const mapDispatchToProps = (dispatch, ownProps) => {
    return {
        callUserInfo: () => dispatch(userInfoAction())
    }
}

export const Header = connect(mapStateToProp, mapDispatchToProps)(HeaderComponent);