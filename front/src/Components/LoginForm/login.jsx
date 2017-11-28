import React from 'react'
import connect from "react-redux/es/connect/connect";
import { bindActionCreators } from 'redux';
import { CALL_API } from "redux-api-middleware";
import rest from '../../common/rest';
import { push } from 'react-router-redux';
import { Redirect } from 'react-router';
import { LoadingComponent } from '../LoadingComponent/index';

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

    onDoLogin = (doLogin) => {
        return (e) => {
            e.preventDefault();
            doLogin(this.state.name)
        }
    }

    componentWillUpdate(nextProps, nextState) {
        let { succeeded, redirectToGameRoom } = nextProps;
        if (succeeded) {
            redirectToGameRoom();
        }
    }

    render() {
        let { doLogin, redirectToGameRoom, succeeded, requestInProgress = false, history } = this.props;
        return <div>
            <form onSubmit={this.onDoLogin(doLogin)}>
                <label htmlFor="name">
                    Name:
                <input type="text" value={this.state.name} onChange={this.onChange} id="name" />
                </label>
                <br />
                <button type="submit">
                    Login
            </button>
            </form >
            {requestInProgress && <LoadingComponent />}
        </div>
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
            return {
                ...action.payload,
                requestInProgress: false
            };
        case LOGIN_FAILURE:
            return state;
        case LOGIN_REQUEST:
            return {
                ...state,
                requestInProgress: true
            }
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
            ownProps.history.history.push('/gameroom')
        }
    };
};

const mergeProps = (stateProps, dispatchProps, ownProps) => {
    return {
        ...stateProps,
        ...dispatchProps,
        ...ownProps
    };
}

const AppLoginForm = connect(mapStateToProps, mapDispatchToProps, mergeProps)(LoginForm);
export default AppLoginForm;
