import React, { useEffect, Fragment } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';

const EmailConfirmed = ({ auth, history }) => {
	useEffect(() => {
		setTimeout(() => history.push('/posts'), 1700);
	}, []);

	return (
		<Fragment>
			{auth.isAuthenticated &&
			auth.loading === false && (
				<div>
					<h1>Welcome {auth.user.username}! Thanks for confirming your email.</h1>
					<small>(You will be redirected shortly)</small>
				</div>
			)}
		</Fragment>
	);
};

EmailConfirmed.propTypes = {
	auth: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	auth: state.auth
});

export default connect(mapStateToProps, {})(EmailConfirmed);
