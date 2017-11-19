import React, { Component } from 'react';
import styles from './scss/index';

const PlayCardSuit = {
    Clubs: 'club',
    Diamonds: 'diamond',
    Hearts: 'heart',
    Spades: 'spade'
}

const RankStylesDictionary = {
    One: 1,
    Two: 2,
    Three: 3,
    Four: 4,
    Five: 5,
    Six: 6,
    Seven: 7,
    Eight: 8,
    Nine: 9,
    Ten: 10,
    Jack: 'J',
    Queen: 'Q',
    King: 'K',
    Ace: 'A'
}

export class EmptyCardSet extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        let { count } = this.props;
        let renderedCards = [];
        for (let i = 0; i < count; i++) {
            renderedCards.push(<div className={`${styles.playcard}`}
                key={i} >
                    <div className={`${styles.empty_card}`} />
                </div>);
        }
        return <div className={styles.cardset}>
            <div className={styles.cardwrapper}>
                {renderedCards}
            </div>
        </div>
    }
}

export class PlayCardSet extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return <div className={styles.cardset}>
            <div className={styles.cardwrapper}>
                {this.props.children}
            </div>
        </div>
    }
}

export class PlayCard extends Component {
    constructor(props) {
        super(props);
    }

    renderCardBody = () =>
        <div className={styles.card__inner}>
            <div className={`${styles.card__column} ${styles.card__columncentered}`}>
                <div className={styles.card__symbol}></div>
            </div>
        </div>;


    render() {
        let { card = {} } = this.props;
        return <div className={styles.playcard}>
            <section className={`${styles.card} ${styles.card}${PlayCardSuit[card.SuitStr]}`}
                value={RankStylesDictionary[card.RankStr]}>
                {this.renderCardBody()}
            </section>
        </div>
    }

}