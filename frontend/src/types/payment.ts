export enum PaymentStatus {
  Pending = "Pending",
  Success = "Success",
  Failed = "Failed",
  Expired = "Expired",
}

export interface PaymentTransaction {
  token: string;
  amount: number;
  status: PaymentStatus;
  reservationNumber: string;
  redirectUrl: string;
  rrn: string | null;
}

export interface UpdateStatusPayload {
  token: string;
  isSuccess: boolean;
  rrn?: string;
}
