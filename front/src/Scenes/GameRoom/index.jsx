import React, { Component } from 'react'
import { WEBSOCKET_CONNECTION_REQUEST } from '../../middleware/websocket';
import { createConnectedComponent } from './Container/index';
import GamePlace from './GamePlace/index';
import styles from './styles/index';
import { EmptyCardSet } from '../../Components/PlayCard/playcartd';
import { PageWrapper } from '../../Components/PageWrapper/index';
import { Modal, ModalHeader, ModalContent } from '../../Components/Modal/index';


class GameRoom extends Component {
    constructor(props) {
        super(props);
        this.connectToWebsocket = props.connectToWebsocket.bind(this);
        this.sendMessage = props.sendMessage.bind(this);
        this.sitOnPlace = this.sitOnPlace.bind(this);
        this.acceptCard = this.acceptCard.bind(this);
        this.declineCard = this.declineCard.bind(this);
        this.state = {
            modalOpen: false
        }
        this.closeModal = this.closeModal.bind(this);
    }

    componentWillMount() {
        this.connectToWebsocket();
    }

    sitOnPlace(placeNumber) {
        this.sendMessage({
            messageType: 'sitOnPlace',
            place: placeNumber
        })
    }

    acceptCard() {
        this.sendMessage({
            messageType: 'acceptCart'
        })
    }

    declineCard() {
        this.sendMessage({
            messageType: 'declineCart'
        })
    }

    renderWinnersModalModal(winner = { Winners: [] }, state, closeModal) {
        // Winner : {"Winners":[{"Name":"igor","CountOfCards":3}],"Sum":20}
        return <Modal isOpen={state.modalOpen} onModalClose={closeModal}>
            <ModalHeader title="Blackjack Game Results" onModalClose={closeModal} />
            <ModalContent>
                <table>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Count of cards</th>
                            <th>Sum</th>
                        </tr>
                    </thead>
                    <tbody>
                        {winner.Winners.map(win => <tr>
                            <td>{win.Name}</td>
                            <td>{win.CountOfCards}</td>
                            <td>{winner.Sum}</td>
                        </tr>)}
                    </tbody>
                </table>
            </ModalContent>
        </Modal>
    }

    closeModal() {
        this.setState({ modalOpen: false })
    }

    componentWillReceiveProps(nextProps) {
        let { winner } = nextProps;
        if (winner != null) {
            this.setState({ modalOpen: true });
        }
    }

    render() {
        let { gameState = {}, currentUser = {}, winner } = this.props;
        let { Users: users = [], CurrentUser: moveOfPlayer = {} } = gameState;
        let winnerModal = this.renderWinnersModalModal(winner, this.state, this.closeModal);
        if (!moveOfPlayer) {
            moveOfPlayer = {};
        }
        let renderedUsers = [];
        for (let key in users) {
            let user = users[key];
            renderedUsers.push(<li className={styles.gameplace} key={key}>
                <GamePlace sitOnPlace={this.sitOnPlace} placeNumber={key} user={user}
                    currentUser={key == currentUser.place ? currentUser : null}
                    acceptCard={this.acceptCard}
                    declineCard={this.declineCard}
                    enableMove={moveOfPlayer.Name == currentUser.name}
                    userInGame={currentUser.name} />
            </li>);
        }
        renderedUsers.push(<li className={styles.gameplace} key="empty-cards"
            style={{ top: "37%", left: "37%" }}>
            <EmptyCardSet count={10} />
        </li>)
        return <PageWrapper>
            <div className={styles.gamewrapper}>
                <ul className={styles.gameroom}>
                    {renderedUsers}
                </ul>
            </div>
            {winnerModal}
        </PageWrapper>
    }
}

export const ONMESSAGE_ACTION_TYPE = "ONMESSAGE_ACTION_TYPE";

export const gameRoomReducer = (state = {}, action) => {
    switch (action.type) {
        case ONMESSAGE_ACTION_TYPE:
            let payload = {};
            try {
                payload = JSON.parse(action.payload);
            } catch (e) { }
            return handleOnMessage(state, payload);
        default:
            return state;
    }
};

const handleOnMessage = (state, payload) => {
    console.warn('handleOnMessage payload: ', payload)
    switch (payload.messageType) {
        case 'gameState':
            return {
                ...payload
            };
        case 'newUser':
            let { Users: users } = state;
            users[payload.place] = {
                Name: payload.name
            }
            return {
                ...state,
                Users: users
            }

        default:
            return state;
    }
}

const AppGameRoom = createConnectedComponent(GameRoom);
export default AppGameRoom;