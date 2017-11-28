import React from 'react';
import { PlayCard, PlayCardSet, EmptyCardSet } from '../../Components/PlayCard/playcartd';
import { Modal, ModalHeader, ModalContent, ModalFooter } from '../../Components/Modal/index';
import { LoadingComponent } from '../../Components/LoadingComponent/index';

export default class Home extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            modalOpen: false,
            loader: false
        }
    }
    render() {
        let onModalClose = _ => {
            this.setState({
                ...this.state,
                modalOpen : false
            });
        }
        let onLoaderClose = _ => {
            this.setState({
                ...this.state,
                loader : true
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
            {this.state.loader && <LoadingComponent/> }
            <button onClick={_ => {
                this.setState({
                    modalOpen : true
                })
            }}>Click to open modal!</button>
            <br/>
            <br/>
            <button onClick={onLoaderClose}>Open loader</button>
        </div>
    }
}