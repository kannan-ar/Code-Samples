import { Purchase } from '@/Models/Purchase';
import { IReport } from './IReport'

export class Top10Products implements IReport {
    private records: Map<string, number> = new Map<string, number>();
    private count = 0;

    Log(purchase: Purchase): void {
        if (!purchase.product.name) return;

        if (this.records.has(purchase.product.name)) {
            this.records.set(purchase.product.name, this.records.get(purchase.product.name) ?? 0 + 1);
        }
        else {
            this.records.set(purchase.product.name, 1);
        }

        if (this.count == 0) {
            console.log(this.records)
        }
    }
}