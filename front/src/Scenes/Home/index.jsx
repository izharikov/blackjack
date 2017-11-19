import React from 'react';
import { PlayCard, PlayCardSet, EmptyCardSet } from '../../Components/PlayCard/playcartd';
import { Modal, ModalHeader, ModalContent, ModalFooter } from '../../Components/Modal/index';

export default class Home extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            modalOpen: false
        }
    }
    render() {
        let onModalClose = _ => {
            this.setState({
                modalOpen : false
            });
        }
        return <div>
            <Modal isOpen={this.state.modalOpen} onModalClose={onModalClose}>
                <ModalHeader title="The example modal" onModalClose={onModalClose}/>
                <ModalContent>
                    Hello, world!
                </ModalContent>
                {/* <ModalFooter/> */}
            </Modal>
            <button onClick={_ => {
                this.setState({
                    modalOpen : true
                })
            }}>Click to open modal!</button>
        </div>
    }
}