import React, { Component } from 'react';
import EmptyGamePlace from './EmptyGamePlace/index';
import UserGamePlace from './UserGamePlace/index';
import { CurrentUserGamePlace } from './CurrentUserGamePlace/index';

export default class GamePlace extends Component {
    constructor(props) {
        super(props);
        this.state = {
            name: '',
            place: ''
        }
        this.onChangeName = this.onChangeName.bind(this);
        this.onChangePlace = this.onChangePlace.bind(this);
    }

    onChangeName(e) {
        this.setState({
            ...this.state,
            name: e.target.value
        })
    }

    onChangePlace(e) {
        this.setState({
            ...this.state,
            place: e.target.value
        })
    }

    renderUser(placeNumber, user, currentUser = {}, props) {
        let { declineCard, acceptCard, enableMove } = props;
        return currentUser != null && placeNumber == currentUser.place ?
            <CurrentUserGamePlace currentUser={currentUser}
                declineCard={declineCard} acceptCard={acceptCard} enableMove={enableMove} /> :
            <UserGamePlace user={user} />;
    }

    render() {
        let { name, place } = this.state;
        let { placeNumber, user = null, sitOnPlace, currentUser, userInGame } = this.props;
        return user == null ?
            <EmptyGamePlace placeNumber={placeNumber} sitOnPlace={sitOnPlace} curUser={userInGame}/>
            : this.renderUser(placeNumber, user, currentUser, this.props)
    }
}