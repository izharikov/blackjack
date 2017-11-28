import React from 'react';
import AppLoginForm from "../../Components/LoginForm/login";
import {Link} from 'react-router-dom';
import { AppRegisterForm } from '../../Components/RegisterForm/index';
import { PageWrapper } from '../../Components/PageWrapper/index';


export class LoginPage extends React.Component {
    render() {
        return <PageWrapper>
            <AppLoginForm history={this.props}/>
            <br/>
            <Link to="/">Go home</Link><br/>
            <Link to="/register">Register</Link>
        </PageWrapper>
    }
}

export class RegisterPage extends React.Component{
    render() {
        return <PageWrapper>
            <AppRegisterForm />
            <br/>
            <Link to="/">Go home</Link><br/>
            <Link to="/login">Login</Link>
        </PageWrapper>
    }
}