import React, {Component} from 'react'
import connect from "react-redux/es/connect/connect";
import { CALL_API } from "redux-api-middleware";
import rest from '../../common/rest';
import {RegisterPage} from '../../Scenes/Authentification/index';
import { push } from 'react-router-redux';

export class RegisterForm extends Component{
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
        let { doRegister, redirectToGameRoom } = this.props;
        return <form onSubmit={(e) => {
            e.preventDefault();
            doRegister(this.state.name)
        }}>
            <label htmlFor="name">
                Name:
                <input type="text" value={this.state.name} onChange={this.onChange} id="name" />
            </label>
            <br/>
            <button type="submit">
                Register
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

const REGISTER_SUCCESS = "REGISTER_ACTION";
const REGISTER_FAILURE = "REGISTER_FAILURE";
const REGISTER_REQUEST = "REGISTER_REQUEST";

/*
 actions
 */

const actions = {
    register: (name) => ({
        [CALL_API]: {
            endpoint: rest.registerUrl,
            method: 'POST',
            body: JSON.stringify({ name }),
            headers: { 'Content-Type': 'application/json' },
            types: [REGISTER_REQUEST, REGISTER_SUCCESS, REGISTER_FAILURE],
            credentials: 'same-origin'
        }
    })
};

/**
 * reducers
 */

export const registerReducer = (state = {}, action) => {
    switch (action.type) {
        case REGISTER_SUCCESS:
            console.log(state, action.payload);
            return state;
        case REGISTER_FAILURE:
            console.warn("Register failure", action);
            return state;
        default:
            return state;
    }
};


const mapStateToProps = (state, ownProps) => {
    return {};
};

const mapDispatchToProps = (dispatch, ownProps) => {
    return {
        doRegister: (name) => {
            dispatch(actions.register(name))
        },
        redirectToGameRoom: () => {
            dispatch(push('/gameroom'))
        }
    };
};

export const AppRegisterForm = connect(mapStateToProps, mapDispatchToProps)(RegisterForm);
