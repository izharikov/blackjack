import React from 'react';
import { PlayCard, PlayCardSet, EmptyCardSet } from '../../Components/PlayCard/playcartd';

export default class Home extends React.Component {
    render() {
        return <div>
            <PlayCardSet>
                <PlayCard card={{ RankStr: 'Ace', SuitStr: 'Spades' }} />
                <PlayCard card={{ RankStr: 'Two', SuitStr: 'Hearts' }} />
                <PlayCard card={{ RankStr: 'Jack', SuitStr: 'Diamonds' }} />
                <PlayCard card={{ RankStr: 'Queen', SuitStr: 'Clubs' }} />
            </PlayCardSet>
            <br/>
            <EmptyCardSet count={4}/>
        </div>
    }
}