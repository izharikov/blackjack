import React from 'react'
import connect from "react-redux/es/connect/connect";
import { bindActionCreators } from 'redux';
import { CALL_API } from "redux-api-middleware";
import rest from '../../common/rest';
import { push } from 'react-router-redux';
import { Redirect } from 'react-router';

export class LoginForm extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            name: ''
        }
        this.onChange = this.onChange.bind(this);
    }

    onChange(e) {
        this.setState({
            name: e.target.value
        })
    }

    render() {
        let { doLogin, redirectToGameRoom, succeeded } = this.props;
        if (succeeded) {
            return <Redirect to="/gameroom"/>
        }
        return <form onSubmit={(e) => {
            e.preventDefault();
            doLogin(this.state.name)
        }}>
            <label htmlFor="name">
                Name:
                <input type="text" value={this.state.name} onChange={this.onChange} id="name" />
            </label>
            <br />
            <button type="submit">
                Login
            </button>
        </form >
    }
}

/*
 React-Redux connect section
 */

/**
 * Acion type
 */

const LOGIN_SUCCESS = "LOGIN_ACTION";
const LOGIN_FAILURE = "LOGIN_FAILURE";
const LOGIN_REQUEST = "LOGIN_REQUEST";

/*
 actions
 */

const actions = {
    login: (name) => ({
        [CALL_API]: {
            endpoint: rest.loginUrl,
            method: 'POST',
            body: JSON.stringify({ name }),
            headers: { 'Content-Type': 'application/json' },
            types: [LOGIN_REQUEST, LOGIN_SUCCESS, LOGIN_FAILURE],
            credentials: 'same-origin'
        }
    })
};

/**
 * reducers
 */

export const loginReducer = (state = {}, action) => {
    switch (action.type) {
        case LOGIN_SUCCESS:
            console.log(action.payload);
            return action.payload;
        case LOGIN_FAILURE:
            console.warn("Login failure", action);
            return state;
        default:
            return state;
    }
};


const mapStateToProps = (state, ownProps) => {
    return state.loginPage;
};

const mapDispatchToProps = (dispatch, ownProps) => {
    return {
        doLogin: (name) => {
            dispatch(actions.login(name))
        },
        redirectToGameRoom: () => {
            dispatch(push('/gameroom'))
        }
    };
};

const AppLoginForm = connect(mapStateToProps, mapDispatchToProps)(LoginForm);
export default AppLoginForm;
