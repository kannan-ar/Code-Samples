import * as React from "react";

interface ITask {
    AssignedTo: string;
    TaskType: string;
}
interface IState {
    count: number;
    tasks: ITask[]
}
export class Hello extends React.Component<{}, IState> {
    state: IState;

    constructor() {
        super({});
        this.state = { count: 10, tasks: [] };
    }

    
   

    componentWillMount() {
       
    }

    render() {
        return (
            <div>
                <div>{this.state.count}</div>
            </div>
        )
    }
}