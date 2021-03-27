import { Purchase } from "@/Models/Purchase";

export interface IReport {
    Log(purchase: Purchase): void
}