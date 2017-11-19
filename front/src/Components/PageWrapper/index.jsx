import React, { Component } from 'react'
import { Header } from '../Header/index';


export class PageWrapper extends Component {
    constructor(props) {
        super(props);
    }
    render(){
        return <div>
                <Header/>
                {this.props.children}
            </div>
    }
}