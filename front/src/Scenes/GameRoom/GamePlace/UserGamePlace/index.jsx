import React, { Component } from 'react';
import { EmptyCardSet } from '../../../../Components/PlayCard/playcartd';

export default class UserGamePlace extends Component {
    constructor(props){
        super(props);
    }

    render(){
        let {user = {}} = this.props;
        return  <div>
                <h2>{user.Name}</h2>
                <EmptyCardSet count={user.CountOfCards}/>
            </div>;
    }
}