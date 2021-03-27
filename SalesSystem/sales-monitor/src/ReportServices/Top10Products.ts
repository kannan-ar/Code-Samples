import { Purchase } from '@/Models/Purchase';
import {IReport} from './IReport'

export class Top10Products implements IReport {
    private records: Record<string, number> = {}
    private count: number = 0;
    
    Log(purchase: Purchase): void {
        if(!purchase.product.name) return;

        this.records[purchase.product.name] += 1;

        if(this.count == 0) {
            console.log(this.records)
        }
    }
}