export interface InstallationData {
  computerName: string;
  installDate: string; // ISO string
  licenseEnds: string;
  licenseExpired: boolean;
  licenseInstallDate: string;
  licenseKey: string;
  trialEnds: string; // ISO string
  trialExpired: boolean;
  membership: string;
  sites: number;
  email: string;
  price: number;
  subscription: string;
  lastUpdate: string;
}

export interface VerifyInstallationResult {
  validLicense: boolean;
  licensed: boolean;
}

export interface InstallationSession {
  product: string;
  email: string;
  price: number;
  expiration: string;
  membership: string;
  sites: number;
}

export interface LicenseValidationResult {
  message: string;
  status: 'success' | 'error';
  alertStyle: 'success' | 'error';
}


export interface GumroadPurchase {
  seller_id: string;
  product_id: string;
  product_name: string;
  permalink: string;
  product_permalink: string;
  email: string;
  price: number;
  gumroad_fee: number;
  currency: string;
  quantity: number;
  discover_fee_charged: boolean;
  can_contact: boolean;
  referrer: string;
  card: {
    visual: string | null;
    type: string | null;
  };
  order_number: number;
  sale_id: string;
  sale_timestamp: string; // ISO date string
  purchaser_id: string;
  subscription_id: string;
  variants: string;
  license_key: string;
  is_multiseat_license: boolean;
  ip_country: string;
  recurrence: string;
  is_gift_receiver_purchase: boolean;
  refunded: boolean;
  disputed: boolean;
  dispute_won: boolean;
  id: string;
  created_at: string; // ISO date string
  custom_fields: any[]; // Assuming custom fields can be of any type
  chargebacked: boolean | null; // Could be null
  subscription_ended_at: string | null; // Could be null
  subscription_cancelled_at: string | null; // Could be null
  subscription_failed_at: string | null; // Could be null
}

export interface GumroadApiResponse {
  success: boolean;
  uses: number;
  purchase: GumroadPurchase;
}
