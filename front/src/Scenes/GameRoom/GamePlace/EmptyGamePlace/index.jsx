import React, { Component } from 'react';
import styles from './scss/emptygameplace';

export default class EmptyGamePlace extends Component {
    constructor(props) {
        super(props);
        this.state = {
            name: ''
        }
        this.onChangeName = this.onChangeName.bind(this);
    }

    onChangeName(e) {
        this.setState({
            name: e.target.value
        })
    }

    render() {
        let { name } = this.state;
        let { placeNumber, sitOnPlace, curUser } = this.props;
        return curUser ? <div /> : <div className={styles.place}>
            <div className={styles.inner_middle}>
                <button onClick={_ => sitOnPlace(placeNumber)}>+</button>
                <p>Sit here</p>
            </div>
        </div>;
    }
}