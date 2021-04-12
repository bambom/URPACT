/*
 *  CBiOSStoreManager.mm
 *  CloudBox Cross-Platform Framework Project
 *
 *  Created by Cloud on 2012/10/30.
 *  Copyright 2011 Cloud Hsu. All rights reserved.
 *
 */

#import "IAP.h"

@implementation CBiOSStoreManager

static CBiOSStoreManager* _sharedInstance = nil;

extern "C" {
    
    bool _CanPurchase()
    {
        return [SKPaymentQueue canMakePayments];
    }
        
    void _Purchase(const char* productId)
    {
        NSLog(@"---------_BuyTest in XCode_------------\n");
        if (_sharedInstance == nil) {
            _sharedInstance = [CBiOSStoreManager sharedInstance];
            NSLog(@"---------__sharedInstance init_------------\n");
            //_sharedInstance = [_sharedInstance init];
            [_sharedInstance initialStore];
        }
        NSLog(@"---------_BuyTest ProductID_IAP0p99------------\n");
        NSString *productIdString = [NSString stringWithCString:productId encoding:NSUTF8StringEncoding];
        [_sharedInstance buy: productIdString];
    }
}



+(CBiOSStoreManager*)sharedInstance
{
	@synchronized([CBiOSStoreManager class])
	{
		if (!_sharedInstance)
			[[self alloc] init];
        
		return _sharedInstance;
	}
	return nil;
}

+(id)alloc
{
	@synchronized([CBiOSStoreManager class])
	{
		NSAssert(_sharedInstance == nil, @"Attempted to allocate a second instance of a singleton.\n");
		_sharedInstance = [super alloc];
		return _sharedInstance;
	}
	return nil;
}

-(id)init {
	self = [super init];
	if (self != nil) {
		// initialize stuff here
	}
	return self;
}

-(void)initialStore
{
    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
}
-(void)releaseStore
{
    [[SKPaymentQueue defaultQueue] removeTransactionObserver:self];
}

-(void)buy:(NSString*)buyProductIDTag
{
    NSLog(@"buy buyProductIDTag: %@\n", buyProductIDTag);
    [self requestProductData:buyProductIDTag];
}

-(bool)CanMakePay
{
    return [SKPaymentQueue canMakePayments];
}

-(void)requestProductData:(NSString*)buyProductIDTag
{
    NSLog(@"---------Request product information------------\n");
    _buyProductIDTag = [buyProductIDTag retain];
    NSArray *product = [[NSArray alloc] initWithObjects:buyProductIDTag,nil];
    NSSet *nsset = [NSSet setWithArray:product];
    SKProductsRequest *request = [[SKProductsRequest alloc] initWithProductIdentifiers: nsset];
    request.delegate = self;
    [request start];
    [product release];
}

- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response{
    
    NSLog(@"-----------Getting product information--------------\n");
    NSArray *myProduct = response.products;
	
	if (myProduct == nil || myProduct.count == 0)
	{
		NSString *aString = @"ProductIsNil";
		UnitySendMessage("MainScript","FailedTransaction",[aString UTF8String]);
		return;
	}
	
    NSLog(@"Product ID:%@\n",response.invalidProductIdentifiers);
    NSLog(@"Product count: %d\n", [myProduct count]);
    // populate UI
    for(SKProduct *product in myProduct){
        NSLog(@"Detail product info\n");
        NSLog(@"SKProduct description: %@\n", [product description]);
        NSLog(@"Product localized title: %@\n" , product.localizedTitle);
        NSLog(@"Product localized descitption: %@\n" , product.localizedDescription);
        NSLog(@"Product price: %@\n" , product.price);
        NSLog(@"Product identifier: %@\n" , product.productIdentifier);
    }
    SKPayment *payment = nil;
    payment = [SKPayment paymentWithProduct:[response.products objectAtIndex:0]];
    NSLog(@"---------Request payment------------\n");
    [[SKPaymentQueue defaultQueue] addPayment:payment];
    [request autorelease];
    
}
- (void)requestProUpgradeProductData:(NSString*)buyProductIDTag
{
    NSLog(@"------Request to upgrade product data---------\n");
    NSSet *productIdentifiers = [NSSet setWithObject:buyProductIDTag];
    SKProductsRequest* productsRequest = [[SKProductsRequest alloc] initWithProductIdentifiers:productIdentifiers];
    productsRequest.delegate = self;
    [productsRequest start];
    
}

- (void)request:(SKRequest *)request didFailWithError:(NSError *)error
{
    NSLog(@"-------Show fail message----------\n");
    UIAlertView *alerView =  [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Alert",NULL) message:[error localizedDescription]
                                                       delegate:nil cancelButtonTitle:NSLocalizedString(@"Close",nil) otherButtonTitles:nil];
    [alerView show];
    [alerView release];
}

-(void) requestDidFinish:(SKRequest *)request
{
    NSLog(@"----------Request finished--------------\n");
    
}

-(void) purchasedTransaction: (SKPaymentTransaction *)transaction
{
    NSLog(@"-----Purchased Transaction----\n");
    NSArray *transactions =[[NSArray alloc] initWithObjects:transaction, nil];
    [self paymentQueue:[SKPaymentQueue defaultQueue] updatedTransactions:transactions];
    [transactions release];
}

- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
    NSLog(@"-----Payment result--------\n");
    for (SKPaymentTransaction *transaction in transactions)
    {
        switch (transaction.transactionState)
        {
            case SKPaymentTransactionStatePurchased:
            {
                [self completeTransaction:transaction];
                NSLog(@"-----Transaction purchased--------\n");
                /*
					UIAlertView *alerView =  [[UIAlertView alloc] initWithTitle:@"Congratulation"
																		message:@"Transaction suceed!"
																	   delegate:nil cancelButtonTitle:NSLocalizedString(@"Close",nil) otherButtonTitles:nil];

					[alerView show];
					[alerView release];
				*/
            }break;
            case SKPaymentTransactionStateFailed:
            {
                [self failedTransaction:transaction];
                NSLog(@"-----Transaction Failed--------\n");
				/*
					UIAlertView *alerView2 =  [[UIAlertView alloc] initWithTitle:@"Failed"
																		 message:@"Sorry, your transcation failed, try again."
																		delegate:nil cancelButtonTitle:NSLocalizedString(@"Close",nil) otherButtonTitles:nil];
                
					[alerView2 show];
					[alerView2 release];
				*/
            }break;
            case SKPaymentTransactionStateRestored:
            {
                [self restoreTransaction:transaction];
                NSLog(@"----- Already buy this product--------\n");
            }break;
            case SKPaymentTransactionStatePurchasing:
            {
                NSLog(@"-----Transcation puchasing--------\n");
            }break;
            default:
                break;
        }
    }
}

- (void) completeTransaction: (SKPaymentTransaction *)transaction
{
    NSLog(@"-----completeTransaction--------\n");
    // Your application should implement these two methods.
    NSString *product = transaction.payment.productIdentifier;
    if ([product length] > 0) {
        
        NSArray *tt = [product componentsSeparatedByString:@"."];
        NSString *bookid = [tt lastObject];
        if ([bookid length] > 0) {
            [self recordTransaction:bookid];
            [self provideContent:bookid];
        }
    }
    
    NSLog(@"-----completeTransThen SendMessageTogameClient--------\n");
    NSString *newStr = [[NSString alloc] initWithData:transaction.transactionReceipt encoding:NSUTF8StringEncoding];
    UnitySendMessage("MainScript","CompleteTransaction",[newStr UTF8String]);
    
    // Remove the transaction from the payment queue.
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
}

-(void)recordTransaction:(NSString *)product
{
    NSLog(@"-----Record transcation--------\n");
    // Todo: Maybe you want to save transaction result into plist.
}

-(void)provideContent:(NSString *)product
{
    NSLog(@"-----Download product content--------\n");
}

- (void) failedTransaction: (SKPaymentTransaction *)transaction
{
    NSLog(@"Failed\n");
    NSString *aString = [NSString stringWithFormat:@"%d", transaction.error.code];
    UnitySendMessage("MainScript","FailedTransaction",[aString UTF8String]);
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
}
-(void) paymentQueueRestoreCompletedTransactionsFinished: (SKPaymentTransaction *)transaction
{
    
}

- (void) restoreTransaction: (SKPaymentTransaction *)transaction
{
    NSLog(@"-----Restore transaction--------\n");
}

-(void) paymentQueue:(SKPaymentQueue *) paymentQueue restoreCompletedTransactionsFailedWithError:(NSError *)error
{
    NSLog(@"-------Payment Queue----\n");
}

#pragma mark connection delegate
- (void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data
{
    NSLog(@"%@\n",  [[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding] autorelease]);
}
- (void)connectionDidFinishLoading:(NSURLConnection *)connection
{
    
}

- (void)connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response
{
    switch([(NSHTTPURLResponse *)response statusCode]) {
        case 200:
        case 206:
            break;
        case 304:
            break;
        case 400:
            break;
        case 404:
            break;
        case 416:
            break;
        case 403:
            break;
        case 401:
        case 500:
            break;
        default:
            break;
    }
}

- (void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error
{
    NSLog(@"test\n");
}

-(void)dealloc
{
    [super dealloc];
}

@end