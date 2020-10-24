import React, { Fragment, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { Redirect, withRouter } from 'react-router-dom';
import { connect } from 'react-redux';
import { setAlert } from '../../actions/alert';
import { getPost } from '../../actions/post';
import Spinner from '../layout/Spinner';

const EditPost = ({ getPost, post: { post, loading }, auth: { user }, match }) => {
	const [ loadingPage, setLoadingPage ] = useState(false);

	const [ formData, setFormData ] = useState({
		nameOfDish: '',
		description: '',
		ingredients: '',
		method: '',
		prepTime: '',
		cookingTime: '',
		feeds: '',
		cuisine: ''
	});

	console.log(match.params.id);

	useEffect(
		() => {
			getPost(match.params.id, setLoadingPage);

			setFormData({
				nameOfDish: loading || !post.nameOfDish ? '' : post.nameOfDish,
				description: loading || !post.description ? '' : post.description,
				ingredients: loading || !post.ingredients ? '' : post.ingredients,
				method: loading || !post.method ? '' : post.method,
				prepTime: loading || !post.prepTime ? '' : post.prepTime,
				cookingTime: loading || !post.cookingTime ? '' : post.cookingTime,
				feeds: loading || !post.feeds ? '' : post.feeds,
				cuisine: loading || !post.cuisine ? '' : post.cuisine
			});
		},
		[ loading ]
	);

	const { cuisine, nameOfDish, description, ingredients, method, prepTime, cookingTime, feeds } = formData;

	const onChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

	if (loading) {
		return <Spinner />;
	}

	return user && user.id !== post.userId ? (
		<Redirect to={`/posts/${match.params.id}`} />
	) : (
		<Fragment>
			<h1>{nameOfDish}</h1>
		</Fragment>
	);
};

EditPost.propTypes = {
	getPost: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	auth: state.auth,
	post: state.post
});

export default connect(mapStateToProps, { getPost })(withRouter(EditPost));
