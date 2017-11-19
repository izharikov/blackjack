import React, { Component } from 'react';
import { PlayCard, PlayCardSet } from '../../../../Components/PlayCard/playcartd';
import styles from './scss/index';

export class CurrentUserGamePlace extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        let { currentUser, acceptCard, declineCard, enableMove } = this.props;
        let cards = currentUser.carts.map(card => <PlayCard card={card}
            key={`${card.Suit}${card.Rank}`} />);
        let sum = currentUser.carts.map(c => c.Value).reduce((a, b) => a + b, 0);
        return <div className={styles.userPlaceWrapper}>
            <div className={styles.vericleMiddle}>
                <div className={styles.userPlace}>
                    <p className={styles.username}>{currentUser.name}
                        {sum != 0 && `. Sum : ${sum}`}</p>
                    <PlayCardSet>
                        {cards}
                    </PlayCardSet>
                    {enableMove &&
                        <div className={styles.control_btns}>
                            <button onClick={() => acceptCard()}
                                className={`${styles.generate_card_btn} ${styles.accept}`}>✓</button>
                            <button onClick={() => declineCard()}
                                className={`${styles.generate_card_btn} ${styles.decline}`}>×</button>
                        </div>
                    }
                </div>
            </div>
        </div>
    }

}