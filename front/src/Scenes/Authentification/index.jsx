import React from 'react';
import AppLoginForm from "../../Components/LoginForm/login";
import {Link} from 'react-router-dom';
import { AppRegisterForm } from '../../Components/RegisterForm/index';


export class LoginPage extends React.Component {
    render() {
        return <div>
            <AppLoginForm />
            <br/>
            <Link to="/">Go home</Link><br/>
            <Link to="/register">Register</Link>
        </div>
    }
}

export class RegisterPage extends React.Component{
    render() {
        return <div>
            <AppRegisterForm />
            <br/>
            <Link to="/">Go home</Link><br/>
            <Link to="/login">Login</Link>
        </div>
    }
}