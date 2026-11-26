
const cooldownObj = () =>
{
    const MAX_VERIFY_COUNT = 5
    const COOLDOWN_SECONDS = 5
    let countsSoFar = 0
    let coolDownInterval = null

    const reset = () =>
    {
        countsSoFar = 0
        $('#attempts-notice').text('')
        $('#cool-down-btn').prop('disabled', false)
        $('#cooldown-timer').text('')
    }

    return {
        MAX_VERIFY_COUNT,
        COOLDOWN_SECONDS,
        generateAttemptsNotice()
        {
            countsSoFar += 1
            // make sure we don't display a count that goes over our MAX_VERIFY_COUNT
            $('#attempts-notice').text(`${Math.min(countsSoFar, MAX_VERIFY_COUNT)} of ${MAX_VERIFY_COUNT} attempts`)

            if (countsSoFar >= MAX_VERIFY_COUNT)
            {
                let time = COOLDOWN_SECONDS
                if (!coolDownInterval)
                {
                    $('#cool-down-btn').prop('disabled', true)
                    coolDownInterval = setInterval(() =>
                    {
                        if (time <= 0)
                        {
                            clearInterval(coolDownInterval)
                            coolDownInterval = null
                            reset()
                            return
                        }
                        $('#cooldown-timer').text(`Please try again after ${time--} seconds`)
                    }, 1000);
                }
            }
        },
    }
}


window.cooldown = cooldownObj()


